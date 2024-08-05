  using HR.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Models
{
    [Table(name: "structural_division", Schema = "public")]
    public class StructuralDivision : HrBase
    {
        [Column("id_department")]
        [Key]
        public override int Id { get; set; }

        [Column("department_code")]
        [JsonProperty(nameof(DepartmentCode))]
        [Required]
        [StringLength(4, ErrorMessage = "Длина поля не должна превышать 4 символов")]
        public string DepartmentCode { get; set; }

        [Column("full_title")]
        [JsonProperty(nameof(FullTitle))]
        [Required]
        [StringLength(300, ErrorMessage = "Длина поля не должна превышать 300 символов")]
        public string FullTitle { get; set; }

        [Column("abbreviation")]
        [JsonProperty(nameof(Abbreviation))]
        [Required]
        [StringLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        public string Abbreviation { get; set; }

        [Column("date_creation")]
        [JsonProperty(nameof(DateCreation))]
        [Remote(action: "CheckDateEndMoreThenStartForDivision", controller: "Check",
            ErrorMessage = "Дата создания должности должна быть меньше даты ликвидации",
            AdditionalFields = nameof(DateLiquidation))]
        public DateTime DateCreation { get; set; }

        [Column("date_liquidation")]
        [JsonProperty(nameof(DateLiquidation))]
        [Remote(action: "CheckDateEndMoreThenStartForDivision", controller: "Check",
            ErrorMessage = "Дата ликвидации должности должна быть больше даты создания",
            AdditionalFields = nameof(DateCreation))]
        public DateTime DateLiquidation { get; set; } = new DateTime(2999, 01, 01);

        [Column("id_position_chief")]
        [JsonProperty(nameof(PositionChiefId))]
        [ForeignKey(nameof(PositionChief))]
        public int? PositionChiefId { get; set; }
        [JsonProperty(nameof(PositionChief))]
        public Position PositionChief { get; set; }

        [Column("id_parent")]
        [JsonProperty(nameof(ParentId))]
        public int ParentId { get; set; } = 1;

        [Column("level")]
        [JsonProperty(nameof(Level))]
        public int Level { get; set; }

        [JsonIgnore]
        public ICollection<Position> Positions { get; set; } = new List<Position>();

        [NotMapped]
        public string DateCreationToString { get { return DateCreation.ToString("d"); } }
    }
}
