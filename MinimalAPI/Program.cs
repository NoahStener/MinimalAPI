
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();


            //Register DB provider
            builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionToDB")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //Endpoint som hämtar alla kuponger
            app.MapGet("/api/coupons", () =>
            {
                APIResponse response = new APIResponse();
                response.Result = CouponStore.couponList;
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;


                return Results.Ok(response);
            }).WithName("GetCoupons").Produces(200);

            

            //Hämta en kupong med ID
            app.MapGet("/api/coupon{id:int}", (int id) =>
            {
                APIResponse response = new APIResponse();

                response.Result = CouponStore.couponList.FirstOrDefault(c => c.ID == id);
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;

                return Results.Ok(response);

            }).WithName("GetCoupon").Produces(200);


            
            //Post endpoint med validering och mapper
            app.MapPost("/api/coupon", async (IValidator<CouponCreateDTO> validator, IMapper _mapper, CouponCreateDTO coupon_C_DTO) =>
            {

                APIResponse response = new() { IsSuccess = false, StatusCode=System.Net.HttpStatusCode.BadRequest };

                var validatorResult = await validator.ValidateAsync(coupon_C_DTO);

                if(!validatorResult.IsValid)
                {
                    return Results.BadRequest(response);
                }
                if(CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
                {
                    response.ErrorMessages.Add("Coupon Name already exists");
                    return Results.BadRequest(response);
                }

                //Med Automapper
                Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
                coupon.ID = CouponStore.couponList.OrderByDescending(c => c.ID).FirstOrDefault().ID + 1;
                CouponStore.couponList.Add(coupon);

                CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
                response.Result = couponDTO;
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.Created;

                return Results.Ok(response);

            }).WithName("CreateCoupon").Produces<CouponCreateDTO>(201)
            .Accepts<APIResponse>("application/json").Produces(400);


            
            //Endpoint för updatering av kuponger
            app.MapPut("/api/coupon", async (IValidator<CouponUpdateDTO> _validator, IMapper _mapper, CouponUpdateDTO coupon_U_DTO) =>
            {

                APIResponse response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };

                //Add validation
                var validateResult = await _validator.ValidateAsync(coupon_U_DTO);
                if (!validateResult.IsValid) 
                {
                    response.ErrorMessages.Add(validateResult.Errors.FirstOrDefault().ToString());
                }

                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.ID == coupon_U_DTO.ID);
                couponFromStore.IsActive = coupon_U_DTO.IsActive;
                couponFromStore.Name = coupon_U_DTO.Name;
                couponFromStore.Percent = coupon_U_DTO.Percent;
                couponFromStore.LastUpdate = DateTime.Now;

                //AutoMapper

                Coupon coupon = _mapper.Map<Coupon>(coupon_U_DTO);

                response.Result = _mapper.Map<CouponDTO>(couponFromStore);
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;

                return Results.Ok(response);

            }).WithName("UpdateCoupon").
            Accepts<CouponUpdateDTO>("application/json").
            Produces<CouponUpdateDTO>(200).Produces(400);


            //Delete endpoint 
            app.MapDelete("/api/coupon{id:int}", (int id) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.ID == id);

                if (couponFromStore != null) 
                {
                    CouponStore.couponList.Remove(couponFromStore);
                    response.IsSuccess = true;
                    response.StatusCode=System.Net.HttpStatusCode.Continue;
                    return Results.Ok(response);
                }
                else
                {
                    response.ErrorMessages.Add("Invalid ID");
                    return Results.BadRequest(response);
                }
            }).WithName("DeleteCoupon");







            app.Run();
        }
    }
}
