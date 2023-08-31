using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Implementation;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext context;
    private DbSet<T> entities;
    string errorMessage = string.Empty;

    public Repository(AppDbContext context)
    {
        this.context = context;
        entities = context.Set<T>();
    }
    public IEnumerable<T> GetAll()
    {
        return entities.AsEnumerable();
    }

    public T Get(int? id)
    {
        return entities.SingleOrDefault(s => s.Id == id);
    }
    public void Insert(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        entities.Add(entity);
        context.SaveChanges();
    }

    public bool Update(T entity)
    {
        entities.Update(entity);
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public void Delete(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        entities.Remove(entity);
        context.SaveChanges();
    }

    public bool ProductExists(int id)
    {
        return context.Products.Any(p => p.Id == id);
    }

    // public void AttachProduct(Product product)
    // {
    //     context.Attach(product);
    //     context.Entry(product)
    // }
}