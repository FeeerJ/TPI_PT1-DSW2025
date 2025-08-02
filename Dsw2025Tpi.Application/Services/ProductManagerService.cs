using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductManagerService 
    {
        private readonly IRepository _repository;

        public ProductManagerService(IRepository repository)
        {
            _repository = repository;

        }


     
        public async Task<IEnumerable<ProductModel.ProductResponse>?> GetProducts() 
        {
            var productList = await _repository.GetAll<Product>();
            if(productList == null || !productList.Any()) throw new NotFoundException("No hay productos registrados.");

            var products = productList.Select(product => new ProductModel.ProductResponse(
                    id: product.Id,
                    sku: product.Sku,
                    name: product.Name,
                    currentUnitPrice: product.CurrentUnitPrice,
                    internalCode: product.InternalCode,
                    description: product.Description,
                    stockQuantity: product.StockQuantity,
                    isActive: product.IsActive
                )).ToList();

            return products;
        }

        
        public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request) 
        {
            if (string.IsNullOrWhiteSpace(request.sku) || string.IsNullOrWhiteSpace(request.name) || request.currentUnitPrice < 0)  throw new ArgumentException("Los valores ingresados son invalidos.");
          
            var existe = await _repository.First<Product>(p => p.Sku == request.sku);
            if (existe != null) throw new DuplicatedEntityException($"Ya existe un producto con el sku {request.sku}");
            

            var product = new Product(request.sku, request.name, request.currentUnitPrice, request.internalCode, request.description, request.stockQuantity, request.isActive); 
            await _repository.Add(product);
            return new ProductModel.ProductResponse(product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity, product.IsActive);
        }
      
        
        public async Task<ProductModel.ProductResponseUpdate?> UpdateProduct(ProductModel.ProductRequest request,Guid id)
        {
            var product = await _repository.GetById<Product>(id);
            if (product == null) throw new NotFoundException($"Producto con ID:{id} no encontrado.");
            if (string.IsNullOrWhiteSpace(request.sku)) throw new ArgumentException("El SKU es obligatorio.");
            if (string.IsNullOrWhiteSpace(request.name))throw new ArgumentException("El nombre es obligatorio.");
            if (request.currentUnitPrice < 0)throw new ArgumentException("El precio no puede ser negativo.");
            if(request.stockQuantity < 0) throw new ArgumentException("El precio no puede ser negativo.");
            product.Sku = request.sku;
            product.Name = request.name;
            product.CurrentUnitPrice = request.currentUnitPrice;
            product.InternalCode = request.internalCode;
            product.Description = request.description;
            product.StockQuantity = request.stockQuantity;
            product.IsActive = request.isActive;
            await _repository.Update(product);

            return new ProductModel.ProductResponseUpdate(
                 product.Id,
                 product.Sku,
                 product.Name,
                 product.CurrentUnitPrice,
                 product.InternalCode,
                 product.Description,
                 product.StockQuantity,
                 product.IsActive
                
            );

        }


    
        public async Task DisableProduct(Guid id) 
        {
            var product = await _repository.GetById<Product>(id);
            if (product == null) throw new NotFoundException($"Producto con ID:{id} no encontrado.");
            if (!product.IsActive) throw new InvalidOperationException($"El producto con ID: {id} ya se encuentra deshabilitado.");

            product.IsActive = false;
            await _repository.Update(product); 

        }

      
        public async Task<ProductModel.ProductResponseUpdate?> GetProductID(Guid id)
        {
            var product = await _repository.First<Product>(p => p.Id == id && p.IsActive == true);
            if (product == null) throw new NotFoundException($"Producto con ID:{id} no encontrado.");
            return new ProductModel.ProductResponseUpdate(
                product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity, product.IsActive 
             );
        }

    }
}
