﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PrimeVilla_Utility;
using PrimeVilla_Web.Models;
using PrimeVilla_Web.Models.DTO;
using PrimeVilla_Web.Models.VM;
using PrimeVilla_Web.Services;
using PrimeVilla_Web.Services.IServices;

namespace PrimeVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();

            var response = await _villaNumberService.GetAllAsync<APIResponse>();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number Created Successfully";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages!= null && response.ErrorMessages.Count>0) 
                        ? response.ErrorMessages[0] 
                        : "Error Encountered";
                }

            }
            var villaResponse = await _villaService.GetAllAsync<APIResponse>();
            if (villaResponse != null && villaResponse.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(villaResponse.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            TempData["error"] = "Error encountered!";
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int VillaNo)
        {
            VillaNumberUpdateVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(VillaNo);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
            }
            else
                return NotFound();

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(villaNumberVM);
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0)
                    ? response.ErrorMessages[0]
                    : "Error Encountered";
            }
            return NotFound();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number Updated Successfully";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0)
                        ? response.ErrorMessages[0]
                        : "Error Encountered";
                }
            }
            var villaResponse = await _villaService.GetAllAsync<APIResponse>();
            if (villaResponse != null && villaResponse.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(villaResponse.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int VillaNo)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(VillaNo);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberVM.VillaNumber = model;
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(villaNumberVM);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {
            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa Number Deleted Successfully";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0)
                    ? response.ErrorMessages[0]
                    : "Error Encountered";
            }

            var villaResponse = await _villaService.GetAllAsync<APIResponse>();
            if (villaResponse != null && villaResponse.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(villaResponse.Result.ToString()).Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            TempData["error"] = "Error encountered!";
            return View(model);
        }
    }
}
