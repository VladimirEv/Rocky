using System.ComponentModel.DataAnnotations;

namespace Rocky.Models
{
    public class SoilProperties
    {
        [Key]
        public int Id { get; set; }

        public int NumberOfSoil { get; set; }                   //номер грунта по геологии

        public string Name { get; set; }

        public string SoilStrengthForSand { get; set; }        //прочный ...

        public double PorosityCoefficient { get; set; }        //e

        public double YieldRate { get; set; }                  // IL

        public double AngleOfInternalFriction { get; set; }    // фи

        public double SpecificAdhesion { get; set; }           // с

        public double DeformationModulus { get; set; }         // E

    }
}
