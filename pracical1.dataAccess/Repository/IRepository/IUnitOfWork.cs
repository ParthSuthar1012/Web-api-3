﻿namespace Repository.Repository.IRepository
{
    public interface IUnitOfWork
    {
     

        ICategoryRepository CategoryRepository { get; }

        IProductRepository productRepository { get; }

        IOrderRepository orderRepository { get; }

        IOrderItemRepository orderItemRepository { get; }
        void Save();
    }
}
