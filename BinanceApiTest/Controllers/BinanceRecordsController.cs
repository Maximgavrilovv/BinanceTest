using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BinanceApiTest.Data;
using BinanceApiTest.Models;

namespace BinanceApiTest.Controllers
{
    public class BinanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BinanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BinanceRecords
        public async Task<IActionResult> Index()
        {
            return View(await _context.BinanceRecords.ToListAsync());
        }

        // GET: BinanceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binanceRecord = await _context.BinanceRecords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binanceRecord == null)
            {
                return NotFound();
            }

            return View(binanceRecord);
        }

        // GET: BinanceRecords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BinanceRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pair,Price")] BinanceRecord binanceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(binanceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(binanceRecord);
        }

        // GET: BinanceRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binanceRecord = await _context.BinanceRecords.FindAsync(id);
            if (binanceRecord == null)
            {
                return NotFound();
            }
            return View(binanceRecord);
        }

        // POST: BinanceRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Pair,Price")] BinanceRecord binanceRecord)
        {
            if (id != binanceRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(binanceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BinanceRecordExists(binanceRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(binanceRecord);
        }

        // GET: BinanceRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binanceRecord = await _context.BinanceRecords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binanceRecord == null)
            {
                return NotFound();
            }

            return View(binanceRecord);
        }

        // POST: BinanceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var binanceRecord = await _context.BinanceRecords.FindAsync(id);
            _context.BinanceRecords.Remove(binanceRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BinanceRecordExists(int id)
        {
            return _context.BinanceRecords.Any(e => e.Id == id);
        }
    }
}
