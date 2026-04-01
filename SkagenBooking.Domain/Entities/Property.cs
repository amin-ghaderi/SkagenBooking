namespace SkagenBooking.Core.Entities;

public class Property
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string City { get; private set; }
    public int ParkingCapacity { get; private set; }

    public Property(int id, string name, string city, int parkingCapacity)
    {
        Id = id;
        Name = name;
        City = city;
        ParkingCapacity = parkingCapacity;
    }
}
