using HR.Abstract;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Models
{
    [Table(name: "position", Schema = "public")]
    public class Position : HrBase
    {
        [Column("id_position")]
        [Key]
        public override int Id { get; set; }

        [Column("name_position")]
        [JsonProperty(nameof(NamePosition))]
        [Required]
        [StringLength(300, ErrorMessage = "Длина поля не должна превышать 300 символов")]
        public string NamePosition { get; set; }

        [Column("date_creation_position")]
        [JsonProperty(nameof(DateCreationPosition))]
        public DateTime DateCreationPosition { get; set; }

        [Column("date_liquidation_position")]
        [JsonProperty(nameof(DateLiquidationPosition))]
        public DateTime DateLiquidationPosition { get; set; }

        [Column("id_department")]
        [JsonProperty(nameof(DepartmentId))]
        [ForeignKey(nameof(StructuralDivision))]
        public int DepartmentId {  get; set; }
        [JsonProperty(nameof(StructuralDivision))]
        public StructuralDivision StructuralDivision { get; set; }

        [JsonProperty(nameof(ChiefForStructuralDivisionId))]
        [NotMapped]
        [ForeignKey(nameof(ChiefForStructuralDivision))]
        public int? ChiefForStructuralDivisionId { get { return DepartmentId; } set { } }
        [JsonProperty(nameof(ChiefForStructuralDivision))]
        public StructuralDivision ChiefForStructuralDivision { get; set; }

        
        [JsonProperty(nameof(Employee))]
        
        public Employee Employee { get; set; }

    }
}
