using Drumpad_Machine.Extra;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Files = System.IO.File;

namespace Drumpad_Machine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetData : ControllerBase
    {
        [HttpGet("{Name}")]
        public async Task<IActionResult> GetSound(string Name)
        {
            try
            {
                var path = AddOns.Path(directory: "Files", filename: Name);
                if (Files.Exists(path))
                {
                    return PhysicalFile(path, "audio/mpeg", Name);
                }
                return StatusCode(403);
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJson()
        {
            try
            {
                return File(await AddOns.GetJsonFileBytes(), "application/json", "SoundLink.json");
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return StatusCode(500);
            }
        }
    }
}
