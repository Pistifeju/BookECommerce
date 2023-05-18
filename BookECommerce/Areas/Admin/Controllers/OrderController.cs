using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;
using BookECommerce.Models.ViewModels;
using BookECommerce.Utility;

namespace BookECommerce.Areas.Admin.Controllers {
    
	[Area("admin")]
    [Authorize]
	public class OrderController : Controller {


		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderViewModel viewModel { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Details(int orderId) {
            viewModel = new() {
                OrderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(viewModel);
        }
        
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail() {
        
            var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == viewModel.OrderHeader.Id);
            orderHeaderFromDb.Name = viewModel.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = viewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = viewModel.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = viewModel.OrderHeader.City;
            orderHeaderFromDb.State = viewModel.OrderHeader.State;
            orderHeaderFromDb.PostalCode = viewModel.OrderHeader.PostalCode;
            
            if (!string.IsNullOrEmpty(viewModel.OrderHeader.Carrier)) {
                orderHeaderFromDb.Carrier = viewModel.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(viewModel.OrderHeader.TrackingNumber)) {
                orderHeaderFromDb.Carrier = viewModel.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new {orderId= orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing() {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(viewModel.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = viewModel.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder() {

            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == viewModel.OrderHeader.Id);
            orderHeader.TrackingNumber = viewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = viewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment) {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = viewModel.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder() {

            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == viewModel.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved) {
                var options = new RefundCreateOptions {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else {
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = viewModel.OrderHeader.Id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW() 
        {
            viewModel.OrderHeader = _unitOfWork.OrderHeaderRepository
                .Get(u => u.Id == viewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
            viewModel.OrderDetail = _unitOfWork.OrderDetailRepository
                .GetAll(u => u.OrderHeaderId == viewModel.OrderHeader.Id, includeProperties: "Product");

            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={viewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={viewModel.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in viewModel.OrderDetail) {
                var sessionLineItem = new SessionLineItemOptions {
                    PriceData = new SessionLineItemPriceDataOptions {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(viewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId) {

            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment) {
                //this is an order by company

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid") {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            
            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
		public IActionResult GetAll(string status) {
            IEnumerable<OrderHeader> objOrderHeaders;


            if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee)) {
                objOrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _unitOfWork.OrderHeaderRepository
                    .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }


            switch (status) {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break; 
            }

            return Json(new { data = objOrderHeaders });
		}

		#endregion
	}
}