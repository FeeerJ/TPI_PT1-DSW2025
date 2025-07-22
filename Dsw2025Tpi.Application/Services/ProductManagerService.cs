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

        public async Task<IEnumerable<Product>?> GetProducts()
        {
            return await _repository.GetAll<Product>();
        }

        public async Task<ProductModel.ProductResponse> AgregarProducto(ProductModel.ProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.sku) || string.IsNullOrWhiteSpace(request.name) || request.currentUnitPrice < 0)
            {
                throw new ArgumentException("Los valores ingresados son invalidos.");
            }
            var existe = await _repository.First<Product>(p => p.Sku == request.sku);
            if (existe != null)
            {
                throw new DuplicatedEntityException($"Ya existe un producot con el sku {request.sku}");
            }

            var producto = new Product(request.sku, request.name, request.currentUnitPrice, request.internalCode, request.description, request.stockQuantity, request.isActive); 
            await _repository.Add(producto);
            return new ProductModel.ProductResponse(producto.Id,producto.Sku, producto.Name, producto.CurrentUnitPrice, producto.InternalCode, producto.Description, producto.StockQuantity, producto.IsActive);
        }
      
        
        public async Task<ProductModel.ProductResponseUpdate?> ModificarProducto(ProductModel.ProductRequest request,Guid id)
        {
            var entidadExistente = await _repository.GetById<Product>(id);
            if (entidadExistente == null) { return null; }
         
            entidadExistente.Sku = request.sku;
            entidadExistente.Name = request.name;
            entidadExistente.CurrentUnitPrice = request.currentUnitPrice;
            entidadExistente.InternalCode = request.internalCode;
            entidadExistente.Description = request.description;
            entidadExistente.StockQuantity = request.stockQuantity;
            entidadExistente.IsActive = request.isActive;


            await _repository.Update(entidadExistente);
            return new ProductModel.ProductResponseUpdate(
                 entidadExistente.Id,
                 entidadExistente.Sku,
                 entidadExistente.Name,
                 entidadExistente.CurrentUnitPrice,
                 entidadExistente.InternalCode,
                 entidadExistente.Description,
                 entidadExistente.StockQuantity,
                 entidadExistente.IsActive
                
                );

        }


        public async Task<bool> DeshabilitarProducto(Guid id)
        {
            var producto = await _repository.GetById<Product>(id);
            if (producto == null || !producto.IsActive== false) 
            {
                throw new Exception();
            }
            producto.IsActive = false;
            await _repository.Update(producto);
            return true;

        }

        public async Task<ProductModel.ProductResponseUpdate?> GetProductID(Guid id)
        {
            var producto = await _repository.First<Product>(p => p.Id == id && p.IsActive == true);
            if (producto == null)
            {
                throw new Exception("erorr bro");
            }

            return new ProductModel.ProductResponseUpdate(
                producto.Id, producto.Sku, producto.Name, producto.CurrentUnitPrice, producto.InternalCode, producto.Description,producto.StockQuantity, producto.IsActive
                
                );
        }

    }
}
