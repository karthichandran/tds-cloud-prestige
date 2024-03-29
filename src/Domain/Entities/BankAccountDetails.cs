﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ReProServices.Domain.Entities
{
    public class BankAccountDetails
    {
        [Key]
        [Column("AccountId")]
        public int AccountId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserPassword { get; set; }

        public string BankName { get; set; }
        public int? LaneNo { get; set; }

        public string LetterA { get; set; }
        public string LetterB { get; set; }
        public string LetterC { get; set; }
        public string LetterD { get; set; }
        public string LetterE { get; set; }
        public string LetterF { get; set; }
        public string LetterG { get; set; }
        public string LetterH { get; set; }
        public string LetterI { get; set; }
        public string LetterJ { get; set; }
        public string LetterK { get; set; }
        public string LetterL { get; set; }
        public string LetterM { get; set; }
        public string LetterN { get; set; }
        public string LetterO { get; set; }
        public string LetterP { get; set; }
    }
}
