using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Well
    {
        public int Id { get; set; }
        public int WellNumber { get; set; }
        public string WellheadRewinding { get; set; }  //отметка устья скважины
        public string WellDepth { get; set; }          //глубина скважины
        public string SoilLayerThickness { get; set; }  // толщина слоя грунта

        [Display(Name = "Soil Properties")]
        public int SoilPropertiesId { get; set; }      //id слоя грунта
        [ForeignKey("SoilPropertiesId")]                            //Чтобы установить свойство в качестве внешнего ключа, применяется атрибут [ForeignKey]:
        public virtual SoilProperties SoilProperties { get; set; }   // грунт

    }
}
