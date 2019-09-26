﻿using System;

namespace Domain.Persistables
{
    public class Document : IPersistableWithName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ShippingId { get; set; }
        public Shipping Shipping { get; set; }

        public Guid FileId { get; set; }
        public FileStorage File { get; set; }

        public Guid TypeId { get; set; }
        public DocumentType Type { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Document document &&
                   Id.Equals(document.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
