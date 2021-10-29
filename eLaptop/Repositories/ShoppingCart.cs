using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLaptop.Data;
using eLaptop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eLaptop.Repositories
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _db;
        private string ShoppingCartId { get; init; }

        public ShoppingCart(ApplicationDbContext db)
        {
            _db = db;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetService<ApplicationDbContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }


        public bool  AddItemToCart(Product product)
        {
            var shoppingCartItem = _db.ShoppingCartItems.FirstOrDefault(n =>
                n.ProductId == product.ProductId && n.ShoppingCartId == ShoppingCartId);
        
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Amount = 1,
                };
                
                _db.ShoppingCartItems.Add(shoppingCartItem);
            }
            else if(shoppingCartItem.Amount < product.Quantity)
            {
                shoppingCartItem.Amount++;
            }
            else
                return false;
            
            _db.SaveChanges();
            return true;
                
        }

        public void RemoveItemFromCart(int productId)
        {
            var shoppingCartItem = _db.ShoppingCartItems.FirstOrDefault(n =>
                n.ProductId == productId && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                    shoppingCartItem.Amount--;
                else
                    _db.ShoppingCartItems.Remove(shoppingCartItem);
            }

            _db.SaveChanges();
        }
        public void DeleteItemFromCart(int id)
        {
            var shoppingCartItem = _db.ShoppingCartItems.FirstOrDefault(n =>
                n.ProductId == id && n.ShoppingCartId == ShoppingCartId);
            
            _db.ShoppingCartItems.Remove(shoppingCartItem);
                    
            _db.SaveChanges();
        }

        public IList<ShoppingCartItem> GetShoppingCartItems(string includeProperties = null)
        {
            IQueryable<ShoppingCartItem> query = _db.ShoppingCartItems;
            
            query = query.Where(n => n.ShoppingCartId == ShoppingCartId);

            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProp) => current.Include(includeProp));
            
            return query.ToList();
        }

        public double GetShoppingCartTotal() => _db.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId)
            .Select(n => n.Price * n.Amount).Sum();

        public async Task ClearShoppingCartAsync()
        {
            var items = await _db.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _db.ShoppingCartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
        }
    }
}