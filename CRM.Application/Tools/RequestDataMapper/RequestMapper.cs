using CRM.Application.Tools.RequestValidation;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Models;

namespace CRM.Application.Tools.RequestDataMapper
{
    /// <summary>
    ///   Provides methods to map request data to application models
    /// </summary>
    public static partial class RequestMapper
    {
        /// <summary>
        ///   Maps the data from the HTTP request to the specified model
        /// </summary>
        /// <returns>The populated model</returns>
        public static User MapToModel(RegistrationRequest request)
        {
            RequestValidator.Validate(request);
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                FatherName = request.FatherName,
                Email = request.Email,
                Password = request.Password,
                Post = request.Post,
                Age = request.Age,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                IsConfirmed = false,
                RegistrationDate = DateTime.UtcNow
            };
        }

        /// <summary>
        ///   Maps the data from the HTTP request to the specified model
        /// </summary>
        /// <returns>The populated model</returns>
        public static User MapToModel(EntityUser user)
        {
            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FatherName = user.FatherName,
                Email = user.Email,
                Password = user.Password,
                Post = user.Post,
                Age = user.Age,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                IsConfirmed = user.IsConfirmed,
                RegistrationDate = user.RegistrationDate
            };
        }

        /// <summary>
        ///   Maps the data from the HTTP request to the specified model
        /// </summary>
        /// <returns>The populated model</returns>
        public static EntityOrder MapToModel(Order order)
        {
            EntityOrder newOrder = new EntityOrder
            {
                Total = order.Total,
                Taxes = order.Taxes,
                PaymentMethod = (int)order.PaymentMethod,
                WorkerId = order.WorkerId,
                OrderСreationDate = order.OrderСreationDate,
                Products = new List<EntityOrderProduct>()
            };

            foreach (var product in order.Products)
            {
                var tempProduct = new EntityOrderProduct
                {
                    ProductId = product.ProductId,
                    Amount = product.Amount,
                    OrderId = newOrder.Id,
                    AddOns = new List<EntityOrderAddOn>()
                };

                foreach (var addOn in product.AddOns)
                {
                    var tempAddOn = new EntityOrderAddOn
                    {
                        AddOnId = addOn.AddOnId,
                        Amount = addOn.Amount,
                        OrderProductId = tempProduct.Id
                    };

                    tempProduct.AddOns.Add(tempAddOn);
                }

                newOrder.Products.Add(tempProduct);
            }

            return newOrder;
        }
    }
}