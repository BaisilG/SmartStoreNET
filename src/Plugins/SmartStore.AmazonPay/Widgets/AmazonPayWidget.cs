﻿using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using SmartStore.Core.Plugins;
using SmartStore.Services.Cms;
using SmartStore.Web.Models.ShoppingCart;

namespace SmartStore.AmazonPay.Widgets
{
	[SystemName("Widgets.AmazonPay")]
	[FriendlyName("Login and Pay with Amazon")]
	public class AmazonPayWidget : IWidget
	{
		private readonly HttpContextBase _httpContext;

		public AmazonPayWidget(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
		}

		public IList<string> GetWidgetZones()
		{
			return new List<string>()
			{
				"order_summary_content_before",
                "offcanvas_cart_summary"
			};
		}

		public void GetDisplayWidgetRoute(string widgetZone, object model, int storeId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			var renderAmazonPayView = true;

			if (widgetZone.IsCaseInsensitiveEqual("offcanvas_cart_summary"))
			{
				actionName = "MiniShoppingCart";

				var viewModel = model as MiniShoppingCartModel;
				if (viewModel != null)
					renderAmazonPayView = viewModel.DisplayCheckoutButton;
			}
			else
			{
				actionName = "OrderReviewData";

				renderAmazonPayView = (_httpContext.HasAmazonPayState() && _httpContext.Request.RequestContext.RouteData.IsRouteEqual("Checkout", "Confirm"));

				if (renderAmazonPayView)
				{
					var viewModel = model as ShoppingCartModel;
					if (viewModel != null)
						viewModel.OrderReviewData.Display = false;
				}
			}

			controllerName = "AmazonPayShoppingCart";

			routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "SmartStore.AmazonPay.Controllers" },
                { "area", AmazonPayPlugin.SystemName },
				{ "renderAmazonPayView", renderAmazonPayView }
            };
		}
	}
}