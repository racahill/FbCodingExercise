using System;

public class CacheItem
{
    //Properties
    public object Value { get; set; }
    public DateTime LastUpdated { get; set; }

    //Constructor
    public CacheItem(object value, DateTime currentDateTime)
    {

        Value = value;
        LastUpdated = currentDateTime;
    }

}
