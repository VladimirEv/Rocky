using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class SoilProperties
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NumberOfSoil { get; set; }                   //номер грунта по геологии

        [Required]
        public string Name { get; set; }

        public string SoilStrengthForSand { get; set; }        //прочный ...

        [Required]
        public string PorosityCoefficient { get; set; }        //e
       
        
        public string YieldRate { get; set; }                  // IL

        
        public float AngleOfInternalFriction { get; set; }    // фи

        
        public float SpecificAdhesion { get; set; }           // с

       
        public float DeformationModulus { get; set; }         // E


        public string NameOfSand { get; set; }                 //песок крупный, средний, мелкий

    }
}
