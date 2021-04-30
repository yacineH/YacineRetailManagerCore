using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Models;
using TRMDesktopUILibrary.API;
using TRMDesktopUILibrary.Helpers;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel :Screen
    {
        private IProductEndPoint _productEndPoint;
        private IConfigHelper _configHelper;
        private ISaleEndPoint _saleEndPoint;
        private IMapper _mapper;
        private StatusInfoViewModel _status;
        private IWindowManager _window; //pour message box (caliburnMicro)

        public  SalesViewModel(IProductEndPoint productEndPoint,
                               IConfigHelper configHelper,
                               ISaleEndPoint saleEndPoint,IMapper mapper,
                               StatusInfoViewModel status,IWindowManager window)
        {
            _productEndPoint = productEndPoint;
            _configHelper = configHelper;
            _saleEndPoint = saleEndPoint;
            _mapper = mapper;
            _status = status;
        }
          

        //c'est une astuce pour ne pas le mette dans le contructeur
        //car le constructeur ne peut pas etre async
        //et pour l'appeler on attend la fin de levenement onload
        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";


                #region version 1 deux dialogBox differentes
                //_status est dans le constructeur
                //premier dialogBox
                //_status.UpdateMessage("Unauthorized access", "you do not have permission to interact with the Sales Form.");
                //_window.ShowDialog(_status, null, settings);
                //TryClose();
                //deuxieme dialoBox qui s'ouvrira apres la fermeture de la premiere
                //_status.UpdateMessage("Unauthorized access", "you do not have permission to interact with the Sales Form.");
                //_window.ShowDialog(_status, null, settings);
                //TryClose(); 
                #endregion

                //version 2ici on peut rajouter plusieurs messages dans une meme dialogBox
                //var info=IoC.Get<StatusInfoViewModel>()

                //version 3 une dialog pour chaque message d'erreur cath
                if(ex.Message=="Unauthorized")
                {
                   _status.UpdateMessage("Unauthorized access", "you do not have permission to interact with the Sales Form.");
                   _window.ShowDialog(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", "you do not have permission to interact with the Sales Form.");
                    _window.ShowDialog(_status, null, settings);
                }
                 TryClose();
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }


        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value;
                NotifyOfPropertyChange(()=>Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(()=>SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
           
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }



        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart=new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity=1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(()=>ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal 
        { 
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subtotal = 0;

            foreach (var item in Cart)
            {
                subtotal += item.Product.RetailPrice * item.QuantityInCart;
            }
            return subtotal;

        }

        public string Tax
        {
            get
            {
                //C =>deux chiffres apres le virgule
                return CalculateTax().ToString("C");
            }
        }


        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;

             taxAmount = Cart
                         .Where(item => item.Product.IsTaxable)
                         .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);


            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}

            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateTax() + CalculateSubTotal();
                return total.ToString("C");
            }
        }



        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                if(ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }
                return output;
            }
        }

        public void AddToCart()
        {
            CartItemDisplayModel existintItem = Cart.FirstOrDefault(x=>x.Product == SelectedProduct );
            if(existintItem!=null)
            {
                existintItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
         
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(()=>SubTotal);
            NotifyOfPropertyChange(()=>Tax);
            NotifyOfPropertyChange(()=>Total);
            NotifyOfPropertyChange(()=>CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        public void RemoveFromCart()
        {

            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;               
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(()=>SubTotal);
            NotifyOfPropertyChange(()=>Tax);
            NotifyOfPropertyChange(()=>Total);
            NotifyOfPropertyChange(()=>CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if(Cart.Count>0)
                {
                    output = true;
                    
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId=item.Product.Id,
                    Quantity=item.QuantityInCart
                });
            }

           await _saleEndPoint.PostSale(sale);

           await ResetSalesViewModel();
        }
    }
}
