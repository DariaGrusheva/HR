using HR.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;


namespace HR.Models
{

    [Table(name: "employee", Schema = "public")]
    public class Employee : HrBase
    {
        [Column("id_employee")]
        [Key]
        public override int Id { get; set; }

        [Column("id_person")]
        [ForeignKey(nameof(Person))]
        [JsonProperty(nameof(PersonId))]
        public int PersonId { get; set; }
        [JsonProperty(nameof(Person))]
        public Person Person { get; set; }

        [Column("service_number")]
        [JsonProperty(nameof(ServiceNumber))]
        public int ServiceNumber { get; set; }

        [Column("date_admission_transfer")]
        [Remote(action: "CheckDateEndMoreThenStartForEmployee", controller: "Check", 
            ErrorMessage = "Дата приема должна быть меньше даты увольнения",
            AdditionalFields = nameof(DateDismissalTransfer))]
        [JsonProperty(nameof(DateAdmissionTransfer))]
        public DateTime DateAdmissionTransfer { get; set; }

        [Column("id_position")]
        [JsonProperty(nameof(PositionId))]
        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }
        [JsonProperty(nameof(Position))]
        public Position Position { get; set; }

        [Column("date_dismissal_transfer")]
        [Remote(action: "CheckDateEndMoreThenStartForEmployee", controller: "Check",
            ErrorMessage = "Дата увольнения должна быть больше даты приема",
            AdditionalFields = nameof(DateAdmissionTransfer))]
        [JsonProperty(nameof(DateDismissalTransfer))]
        public DateTime DateDismissalTransfer { get; set; } = new DateTime(2999, 01, 01);

        [NotMapped]
        public string DateAdmissionTransferToString { get { return DateAdmissionTransfer.ToString("d"); } }
        [NotMapped]
        public string DateDismissalTransferToString { get { return DateDismissalTransfer.ToString("d"); } }

    }
}
