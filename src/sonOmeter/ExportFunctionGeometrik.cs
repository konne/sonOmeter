using sonOmeter.Classes; 
using System.Globalization;
using System; 

class ExportClass
{
	NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;

	long count;
  
	public ExportClass()
	{

  		count = 1;					// Startwert fuer fortlaufende Nummer
  		nfi.NumberDecimalDigits=3;	// 2 Nachkommastellen
	}	

	public string ExportLine(SonarLine line, SonarPanelType type, ExportSettings cfg)
	{
		LineData data;
        bool dataf = false;
        
		switch (type)
		{
			case SonarPanelType.HF:
				data = line.HF;
				break; 
			case SonarPanelType.NF:
				data = line.NF;
				break; 
			default: 
				data = null; 
        	break; 
		}

        double deep=0;
		try
		{         
            int colorid = 0;
			foreach (DataEntry entry in data.Entries)
			{					
				deep = entry.high;

                if (cfg.ExportWithArch)
                {
                    deep = data.Depth;
                    colorid = data.TopColor;                    
                }
                else
                {
                    if (cfg.ExportWithCut)
                    {

                        if ((deep > data.BCut) & (entry.low < data.BCut))
                            deep = data.BCut; // Beschneidungslinie geht mitten durch einen Entry

                        if ((deep <= data.ECut) | (deep >= data.BCut))
                            continue;
                    }
                    colorid = entry.colorID;                   
                }

                if (deep > cfg.ExportMinDeep)
                    continue;

                if (deep < cfg.ExportMaxDeep)
                    continue;

                dataf = true;
				break;
			}
            if (dataf)
            {
                // Variante Geometrik
                string s = "";
                s += count.ToString().PadLeft(10, ' ');	                            // fortlaufenden Nummer
                s += "    1029";                                                    // Punktart (Gewässersohle)
                s += line.CoordRvHv.HV.ToString("F", nfi).PadLeft(15, ' ');         // Hochwert Sonar
                s += line.CoordRvHv.RV.ToString("F", nfi).PadLeft(15, ' ');         // Rechtswert Sonar
                s += (deep + line.CoordRvHv.AL).ToString("F", nfi).PadLeft(9, ' '); // Hoehe Erste Lotung

                count++;
                return s;
            }
            else
            {
                return ""; // keine Daten in der Zeile
            }
		}
		catch
		{
			return ""; // bei Fehler leerer String
		}
	}
}