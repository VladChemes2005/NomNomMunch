using System;

[Serializable]
public class ArrayLayout<T>
{
	[Serializable]
	public struct RowData
	{
		public T[] row;
	}

	public RowData[] rows;

	public ArrayLayout(uint width = 8, uint height = 8)
	{
		rows = new RowData[height];
        for (int i = 0; i < rows.Length; i++)
			rows[i].row = new T[width];
	}
}