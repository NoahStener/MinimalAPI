using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Models.DTOs;
using Newtonsoft.Json;
using Web_Coupon.Models;
using Web_Coupon.Services;

namespace Web_Coupon.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        //Ändrat i denna litegrann
        public async Task <IActionResult> CouponIndex()
        {
            List<CouponDTO> List = new List<CouponDTO>();
            var respons = await _couponService.GetAllCoupon<ResponseDTO>();

            if(respons != null && respons.IsSuccess)
            {
                List = JsonConvert.DeserializeObject
                    <List<CouponDTO>>(Convert.ToString(respons.Result));
            }

            if(List.Count == 0)
            {
                Console.WriteLine("No data recieved from API");
            }
            return View(List);
        }

        public async Task<IActionResult> Details(int id)
        {
            //CouponDTO cDTO = new CouponDTO();

            var respons = await _couponService.GetCouponById<ResponseDTO>(id);

            if(respons != null && respons.IsSuccess)
            {
                CouponDTO model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(respons.Result));

                return View(model);
            }
            return View();
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if(ModelState.IsValid)
            {
                var respons = await _couponService.CreateCouponAsync<ResponseDTO>(model);
                if(respons != null && respons.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult>UpdateCoupon(int couponId)
        {
            var response = await _couponService.GetCouponById<ResponseDTO>(couponId);

            if(response != null && response.IsSuccess)
            {
                CouponDTO model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult>UpdateCoupon(CouponDTO model)
        {
            if(ModelState.IsValid)
            {
                var respons = await _couponService.UpdateCouponAsync<ResponseDTO>(model);

                if(respons != null && respons.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }

        public async Task<IActionResult>DeleteCoupon(int couponId)
        {
            var response = await _couponService.GetCouponById<ResponseDTO>(couponId);

            if (response != null && response.IsSuccess)
            {
                CouponDTO model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.DeleteCouponAsync<ResponseDTO>(model.ID);

                if(response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return RedirectToAction(nameof(CouponIndex));
        }
    }
}
