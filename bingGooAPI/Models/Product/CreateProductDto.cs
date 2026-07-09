using bingGooAPI.Models.ProductScale;

public class CreateProductDto
{
    public int Id { get; set; }

    public string ProNumY { get; set; } = string.Empty;
    public string? ProNumS { get; set; }
    public string? ProNumYP { get; set; }
    public string? ProNumYC { get; set; }

    public string? Sup1 { get; set; }
    public string? Sup2 { get; set; }

    public string? ProName { get; set; }
    public string? KhmerNameUnicode { get; set; }



    public string? ProDes { get; set; }
    public string? ProCat { get; set; }
    public string? ProPacksize { get; set; }

    public string? ProCurr { get; set; }

    // money
    public decimal? ProImpPri { get; set; }

    // decimal
    public decimal? ProRecLev { get; set; }
    public decimal? ProRecOrder { get; set; }

    public string? KhmerName { get; set; }

    //// decimal
    //public decimal? ProSSec { get; set; }

    public string? ProRem { get; set; }

    // nvarchar(10)
    public string? Auto { get; set; }
    public string? ProfitAuto { get; set; }

    // int
    public int? ProTotQty { get; set; }

    public string? ProMadein { get; set; }

    // decimal
    public decimal? ProQtyPCase { get; set; }

    // nvarchar
    public string? ProQtyPPack { get; set; }
    public string? ProPckPri { get; set; }

    // real
    public float? ProPckDis { get; set; }
    public float? ProPckAllDis { get; set; }

    public decimal? ProRecomLev { get; set; }

    // int
    public int? Promotion { get; set; }

    // money
    public decimal? FormDLanded { get; set; }
    public decimal? ProUPriBY { get; set; }

    // real
    public float? ProAllowDisW { get; set; }
    public float? ProAllowDisU { get; set; }

    public float? ProDis { get; set; }

    // float
    public double? ExciseTax { get; set; }
    public double? PublicLightingTax { get; set; }

    // real
    public float? ProVAT { get; set; }

    // money
    public decimal? ProFinBuyin { get; set; }
    public decimal? ProUPrSE { get; set; }

    // real
    public float? ProProPer { get; set; }

    // money
    public decimal? ProUPriSeH { get; set; }

    // real
    public float? ProHolesaleper { get; set; }
    public float? ProHoleSalePP { get; set; }
    public float? ProRecPer { get; set; }

    public string? ProSKU { get; set; }

    // money
    public decimal? Average { get; set; }

    public DateTime? BirthDate { get; set; }

    // decimal
    public decimal? AverSalePmonth { get; set; }

    // decimal
    public decimal? WHcode { get; set; }

    // money
    public decimal? Sampling { get; set; }

    public string? FactoryCurrency { get; set; }
    public string? FOB_CIF { get; set; }

    // money
    public decimal? FOBCIFCost { get; set; }

    // nvarchar(100)
    public string? ShelfLifeOfProduct { get; set; }

    // money
    public decimal? VOP { get; set; }

    // nvarchar(500)
    public string? ProImage { get; set; }

    public ProductScaleDto? ProductScale { get; set; }
}