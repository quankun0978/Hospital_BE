    using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class AllCodeController : BaseController
    {
        private readonly IAllCodeService _allCodeService;

        public AllCodeController(IAllCodeService allCodeService)
        {
            _allCodeService = allCodeService;
        }

        /// <summary>
        /// Lấy danh sách mã theo loại
        /// </summary>
        /// <param name="codeType">Loại mã (ví dụ: ROLE, POSITION, PRICE, TIME, ...)</param>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách mã theo loại</returns>
        [HttpGet("by-type/{codeType}")]
        public async Task<IActionResult> GetAllCodesByType(string codeType, [FromQuery] PaginationParameters parameters)
        {
            var result = await _allCodeService.GetAllCodesByTypeAsync(codeType, parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, $"Lấy danh sách mã loại {codeType} thành công");
        }

        /// <summary>
        /// Lấy danh sách mã theo loại (không phân trang) - dùng cho dropdown
        /// </summary>
        /// <param name="codeType">Loại mã (ví dụ: TIME, ROLE, POSITION, ...)</param>
        /// <returns>Danh sách mã theo loại</returns>
        [HttpGet("options/{codeType}")]
        public async Task<IActionResult> GetAllCodeOptions(string codeType)
        {
            var result = await _allCodeService.GetAllCodeOptionsByTypeAsync(codeType);
            return ApiOk(result, $"Lấy danh sách tùy chọn loại {codeType} thành công");
        }

        /// <summary>
        /// Lấy tất cả loại mã có trong hệ thống
        /// </summary>
        /// <returns>Danh sách các loại mã khác nhau</returns>
        [HttpGet("code-types")]
        public async Task<IActionResult> GetAllCodeTypes()
        {
            var codeTypes = await _allCodeService.GetAllCodeTypesAsync();
            return ApiOk(codeTypes, "Lấy danh sách loại mã thành công");
        }

        /// <summary>
        /// Lấy danh sách tất cả mã
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách tất cả mã</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCodes([FromQuery] QueryParameters parameters)
        {
            var result = await _allCodeService.GetAllCodesAsync(parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, "Lấy danh sách mã thành công");
        }
    }
} 