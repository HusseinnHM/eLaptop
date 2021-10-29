using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
using eLaptop.Data;
using eLaptop.Models;
using eLaptop.ViewModels.Order;
using Microsoft.EntityFrameworkCore;

namespace eLaptop.Repositories
{

    public interface IOrderRepository : IRepository<Order>
    {
        public  Task<IList<Order>> GetOrdersByUserIdAsync(string userId);
        public  Task StoreOrderAsync(Order order,IList<OrderItem> orderItems);
        public Task<IList<Order>> GetAllOrdersAsync();


    }
    public class OrderRepository :Repository<Order> ,  IOrderRepository  
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<IList<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _db.Orders
                .Include(n => n.OrderItems)
                .ThenInclude(n => n.Product)
                .Where(n => n.ApplicationUserId == userId)
                .OrderByDescending(n => n.ShoppingDate)
                .ToListAsync();
            return orders;
        }
           public async Task<IList<Order>> GetAllOrdersAsync()
        {
            var orders = await _db.Orders
                .Include(n=>n.ApplicationUser)
                .Include(n => n.OrderItems)
                .ThenInclude(n => n.Product)
                .Where(n => n.ApplicationUserId == n.ApplicationUser.Id)
                .OrderByDescending(n => n.ShoppingDate)
                .ToListAsync();
            return orders;
        }
        
         public async Task StoreOrderAsync(Order order,IList<OrderItem> orderItems)
         {
             await _db.Orders.AddAsync(order);
           
             await _db.SaveChangesAsync();

            orderItems.ForAll(p => p.OrderId = order.OrderId);
            _db.OrderItems.AddRange(orderItems);
            
            
        }

    }
}