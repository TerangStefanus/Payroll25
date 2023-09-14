﻿using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace Payroll25.Models
{
    public class IdentitasAsistenModel
    {
        [Optional]
        public int? ID_ASISTEN { get; set; }

        [Required]
        [Name("ID_TAHUN_AKADEMIK")]
        public int? ID_TAHUN_AKADEMIK { get; set; }

        [Required]
        [Name("NO_SEMESTER")]
        public int? NO_SEMESTER { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NPM")]
        public string NPM { get; set; }

        [Optional]
        public string NAMA_MHS { get; set; }

        [Required]
        [Name("ID_UNIT")]
        public int? ID_UNIT { get; set; }

        [Optional]
        public string NAMA_UNIT { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NO_REKENING")]
        public string NO_REKENING { get; set; }

        [Required]
        [StringLength(100)]
        [Name("NAMA_REKENING")]
        public string NAMA_REKENING { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NAMA_BANK")]
        public string NAMA_BANK { get; set; }

        [Required]
        [Name("ID_JENIS_ASISTEN")]
        public int? ID_JENIS_ASISTEN { get; set; }

        [Optional]
        public string JENIS { get; set; }
    }

    public class IndexViewModel
    {
        public IEnumerable<IdentitasAsistenModel> IdentitasAsistenList { get; set; }
        public IdentitasAsistenModel IdentitasAsisten { get; set; }
        public List<IdentitasAsistenModel> UnitsList { get; set; }
        public List<IdentitasAsistenModel> JenisAsistenList { get; set; }

        [Required]
        [Display(Name = "CSV File")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "csv", ErrorMessage = "Please upload a valid CSV file.")]
        public IFormFile CsvFile { get; set; }
    }

    public class IdentitasAsistenModelMap : ClassMap<IdentitasAsistenModel>
    {
        public IdentitasAsistenModelMap()
        {
            Map(m => m.ID_TAHUN_AKADEMIK).Name("ID_TAHUN_AKADEMIK");
            Map(m => m.NO_SEMESTER).Name("NO_SEMESTER");
            Map(m => m.NPM).Name("NPM");
            Map(m => m.NAMA_MHS).Name("NAMA_MHS");
            Map(m => m.ID_UNIT).Name("ID_UNIT");
            Map(m => m.NO_REKENING).Name("NO_REKENING");
            Map(m => m.NAMA_REKENING).Name("NAMA_REKENING");
            Map(m => m.NAMA_BANK).Name("NAMA_BANK");
            Map(m => m.ID_JENIS_ASISTEN).Name("ID_JENIS_ASISTEN");
        }
    }




}


