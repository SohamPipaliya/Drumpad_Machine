using Drumpad_Machine.Extra;
using Drumpad_Machine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data = System.IO.File;

namespace Drumpad_Machine.Controllers
{
    public class FileHandlingController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await AddOns.ReadFromJson());
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return View();
            }
        }

        public async Task<IActionResult> Details(Guid id) => View(await SingleAsync(id));

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> files)
        {
            try
            {
                if (files == null)
                    return RedirectToAction(nameof(Index));
                foreach (var file in files)
                {
                    using (var fs = new FileStream(AddOns.Path(directory: "Files", filename: file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                }
                await files.WriteToJson();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return View();
            }
        }

        public async Task<IActionResult> Edit(Guid id) => View(await SingleAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Audio audio)
        {
            try
            {
                if (id != audio.ID)
                    return BadRequest();
                var list = await AddOns.ReadFromJson();
                var solo = list.FirstOrDefault(x => x.ID == id);
                if (Data.Exists(AddOns.Path(directory: "Files", filename: solo.Name)))
                    Data.Move(AddOns.Path(directory: "Files", filename: solo.Name), AddOns.Path(directory: "Files", filename: audio.Name));
                solo.Name = audio.Name;
                solo.Size = audio.Size;
                await list.WriteToJson();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return View();
            }
        }

        public async Task<IActionResult> Delete(Guid id) => View(await SingleAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Audio audio)
        {
            try
            {
                var list = await AddOns.ReadFromJson();
                var newlist = new List<Audio>();
                foreach (var item in list)
                {
                    if (item != audio)
                        newlist.Add(item);
                }
                var name = (await SingleAsync(audio.ID)).Name;
                await newlist.WriteToJson();
                if (Data.Exists(AddOns.Path(directory: "Files", filename: name)))
                    Data.Delete(AddOns.Path(directory: "Files", filename: name));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return View();
            }
        }

        [NonAction]
        private async Task<Audio> SingleAsync(Guid id)
        {
            try
            {
                return (await AddOns.ReadFromJson()).FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
                return new();
            }
        }
    }
}
