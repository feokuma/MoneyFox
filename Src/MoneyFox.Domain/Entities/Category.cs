﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyFox.Domain.Entities
{
    public class Category
    {
        //used by EF Core
        private Category()
        {
        }

        public Category(string name, string note = "")
        {
            CreationTime = DateTime.Now;
            Payments = new List<Payment>();

            UpdateData(name, note);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public string Name { get; private set; }

        private string note;

        public string Note
        {
            private set => note = value;
            get { return note ?? string.Empty; }
        }

        public DateTime ModificationDate { get; private set; }

        public DateTime CreationTime { get; private set; }

        public List<Payment> Payments { get; private set; }

        public void UpdateData(string name, string note = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Note = note;
            ModificationDate = DateTime.Now;
        }
    }
}
