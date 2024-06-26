﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly InventoryDbContext _dbContext;

        public InventoryController(ILogger<InventoryController> logger, InventoryDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> VerifyInventory([FromBody] ItemVerification verification)
        {
            // Add some cpu intensive work
            KillCore();

            var item = await _dbContext.Items.SingleOrDefaultAsync(x => x.ItemCode == verification.ItemCode);
            if(item == null)
                return BadRequest($"Item {verification.ItemCode} not found");
            if(item.TotalQuantity < verification.Quantity) 
                return BadRequest($"Item {verification.ItemCode} doesn't have enough quantity");
            return Ok("Inventory check successful");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var items = await _dbContext.Items.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("{itemCode}")]
        public async Task<IActionResult> GetByItemCode(string itemCode) 
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(x => x.ItemCode == itemCode);
            return Ok(item);
        }

        [HttpPost]
        [Route("claim")]
        public async Task<IActionResult> Claim([FromBody] ItemClaim claim) 
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(x => x.ItemCode == claim.ItemCode);
            if(item == null) return BadRequest($"Item {claim.ItemCode} not found");
            if(item.TotalQuantity < claim.Quantity) return BadRequest($"Item {claim.ItemCode} doesn't have enough quantity");

            item.TotalQuantity = item.TotalQuantity - claim.Quantity;

            await _dbContext.SaveChangesAsync();
            return Ok(item);
        }
        public void KillCore()
        {
            Random rand = new Random();

            Stopwatch watch = new Stopwatch();
            watch.Start();            

            long num = 0;
            while(true)
            {
                num += rand.Next(100, 1000);
                if (num > 1000000) { num = 0; }
                if (watch.ElapsedMilliseconds > 1000) break;
            }
        }

    }
}
