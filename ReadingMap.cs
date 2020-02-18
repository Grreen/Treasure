using System;

public class ReadingMap
{
	private int _widthMap;
	private int _heightMap;

	public ReadingMap(string nameMap)
	{
		if (nameMap == null)
			throw new ArgumentException("nameMap", "ReadingMap says: Parameter 'nameMap' cannot be null!");

		try
        {
			StreamReader str = new StreamReader(nameMap);
		}
		catch (Exception e)
		{
			throw new Exception(e);
		}

		string line;
		while ((line = str.ReadLine()) != null)
		{
			Console.WriteLine(line);
		}

	}

}
