using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Repositories
{/*
    public class InMemory : IRepository
    {
        private List<Product>? _products;

        public InMemory() {
           LoadProducts(); 
        }
        private void LoadProducts()
        {
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Sources\\products.json"));
            _products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


        }

        private List<T>? GetSet<T>() where T: EntityBase /*Devuelve la lista de tipos que queremos en base al tipo que recibe el metodo ObtenerTodo*//*
        {
            if(typeof(T) == typeof(Product))
            {
                return _products as List<T>;
            }
            throw new NotSupportedException();
        }

        public async Task<List<T>?> ObtenerTodo<T>() where T : EntityBase
        {
            return await Task.FromResult(GetSet<T>()?.ToList());
        }

        public async Task<T?> ObtenerPorID<T>(Guid id) where T : EntityBase
        {
            var products = GetSet<T>();
            if (products == null) return null;
            return await Task.FromResult(products.FirstOrDefault(p => p.Id == id));
        }
        
        public async Task<T?> Agregar<T>(T entidad) where T : EntityBase
        {
            GetSet<T>()?.Add(entidad);
            return await Task.FromResult(entidad);
        }

        public async Task<T?> Modificar<T>(T entidad) where T: EntityBase
        {
              var listaProductos = GetSet<T>();
              if (listaProductos == null) return null;

              var entidadExistente  = listaProductos.FirstOrDefault(e => e.Id == entidad.Id);
              if (entidadExistente == null) return null;
              listaProductos.Remove(entidadExistente);
            listaProductos.Add(entidad);
            return await Task.FromResult(entidad);
        }
        public async Task<T?> First<T> (Expression<Func<T,bool>> predicate) where T: EntityBase
        {
            var product = GetSet<T>()?.FirstOrDefault(predicate.Compile());
            return await Task.FromResult(product);
        }

    }*/
}
