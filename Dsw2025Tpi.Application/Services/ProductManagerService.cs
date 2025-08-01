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

        public ProductManagerService(IRepository repository) /*Inyeccion de Dependencia*/
        {
            _repository = repository;

        }


        /*LOGICA PARA OBTENER TODOS LOS PRODUCTOS*/
        public async Task<IEnumerable<Product>?> GetProducts() /*Funcion asincronica que nos permite obtener todos los productos cargados en la bd*/ /*Buscar diferencia entre Enumerable e IEnumerable*/
        {
            return await _repository.GetAll<Product>(); /*Llamamos al repositorio y le decimos que queremos todos los productos de tipo Product*/
        }

        /*LOGICA PARA AGREGAR UN PRODUCTO*/
        public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request) /*Devuelve una respuesta del Modelo Producto*/
        {
            if (string.IsNullOrWhiteSpace(request.sku) || string.IsNullOrWhiteSpace(request.name) || request.currentUnitPrice < 0)  throw new ArgumentException("Los valores ingresados son invalidos.");
          
            var existe = await _repository.First<Product>(p => p.Sku == request.sku); /*Si encuentra una coincidencia respecto al sku perteneciente al producto buscado, se le asigna el obj producto al var*/
            if (existe != null) throw new DuplicatedEntityException($"Ya existe un producto con el sku {request.sku}");
            

            var product = new Product(request.sku, request.name, request.currentUnitPrice, request.internalCode, request.description, request.stockQuantity, request.isActive);  /*Deberia estar en las primeras lineas para que tenga mas sentido visualcreo*/
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


        /* LOGICA PARA DESHABILITAR UN PRODUCTO POR ID*/
        public async Task<bool> DisableProduct(Guid id) 
        {
            var product = await _repository.GetById<Product>(id);  /*Comprueba la existencia del producto que posea la ID que se le envio y la almacena en la variable*/
            if (product == null) throw new NotFoundException($"Producto con ID:{id} no encontrado.");
            if (!product.IsActive) throw new InvalidOperationException($"El producto con ID: {id} ya se encuentra deshabilitado.");

            product.IsActive = false; /*Si el producto existe, se le cambia el estado a false, es decir que se deshabilita*/
            await _repository.Update(product); /*Se actualiza el producto en la base de datos*/
            return true;

        }

        /* LOGICA PARA CONSEGUIR UN PRODUCTO POR SU ID*/
        public async Task<ProductModel.ProductResponseUpdate?> GetProductID(Guid id) /*Si existe va a retornar una respuesta de tipo producto, es decir todos sus datos*/
        {
            var product = await _repository.First<Product>(p => p.Id == id && p.IsActive == true);
            if (product == null) throw new NotFoundException($"Producto con ID:{id} no encontrado."); /*MEJORAR ESTO*/
            return new ProductModel.ProductResponseUpdate(
                product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity, product.IsActive
                 /*Ver si podemos crear una forma mas limpia de devolver esto*/
                );
        }

    }
}
