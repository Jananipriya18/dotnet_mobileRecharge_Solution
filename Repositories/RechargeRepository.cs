// RechargeRepository.cs
using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace dotnetapp.Repositories
{
    public class RechargeRepository : IRechargeRepository
    {
        private readonly ApplicationDbContext _context;

        public RechargeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Recharge AddRecharge(Recharge recharge)
        {
            // Ensure that the associated User and Plan entities exist
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == recharge.UserId);
            var existingPlan = _context.Plans.FirstOrDefault(p => p.PlanId == recharge.PlanId);

            if (existingUser == null || existingPlan == null)
            {
                // Handle the case where User or Plan does not exist
                // You might want to throw an exception or handle it based on your application logic
                // For now, I'm just returning null
                return null;
            }

            // Associate User and Plan with the Recharge entity
            recharge.User = existingUser;
            recharge.Plan = existingPlan;

            // Add and save changes
            _context.Recharges.Add(recharge);
            _context.SaveChanges();

            return recharge;
        }

        public Recharge GetRechargeById(long rechargeId)
        {
            return _context.Recharges
                .Include(r => r.User)
                .Include(r => r.Plan)
                .FirstOrDefault(r => r.RechargeId == rechargeId);
        }

        public List<Recharge> GetRechargesByUserId(long userId)
        {
            return _context.Recharges
                .Include(r => r.User)
                .Include(r => r.Plan)
                .Where(r => r.User.UserId == userId)
                .ToList();
        }

        public List<Recharge> GetAllRecharges()
        {
            return _context.Recharges
                .Include(r => r.User)
                .Include(r => r.Plan)
                .ToList();
        }

        public List<int> GetPricesByUserId(long userId)
        {
            var recharges = _context.Recharges
                .Where(r => r.UserId == userId)
                .Select(r => r.Price)
                .ToList();
            return recharges;
        }
    }
}
