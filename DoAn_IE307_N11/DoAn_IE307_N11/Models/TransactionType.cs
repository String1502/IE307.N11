﻿using SQLite;

namespace DoAn_IE307_N11.Models
{
    public class TransactionType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Image { get; set; }
        public bool IsIncome { get; set; } = true;
    }
}