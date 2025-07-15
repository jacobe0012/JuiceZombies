using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

/// <summary>
/// A circle is a shape consisting of all points in a plane that are at a given distance from a given point, the centre.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct CircleShape : IEquatable<CircleShape>
{
    /// <summary>
    /// Center of the circle.
    /// </summary>
    public float2 Center;

    /// <summary>
    /// Radius of the circle.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Diameter of the circle. Diameter = 2 * Radius. 
    /// </summary>
    public float Diameter
    {
        get => 2f * Radius;
        set => Radius = value * 0.5f;
    }

    /// <summary>
    /// Returns the perimeter of the circle.
    /// The perimeter of a circle is its boundary or the complete arc length of the periphery of a circle.
    /// </summary>
    public float Perimeter => 2f * math.PI * Radius;

    /// <summary>
    /// Returns the area of the circle.
    /// </summary>
    public float Area => math.PI * Radius * Radius;

    public CircleShape(float2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    /// <inheritdoc />
    public bool Equals(CircleShape other) => math.all(Center == other.Center & Radius == other.Radius);

    /// <inheritdoc />
    public override bool Equals(object other) => throw new NotImplementedException();

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc />
    public static bool operator ==(CircleShape lhs, CircleShape rhs) =>
        math.all(lhs.Center == rhs.Center & lhs.Radius == rhs.Radius);

    /// <inheritdoc />
    public static bool operator !=(CircleShape lhs, CircleShape rhs) => !(lhs == rhs);

    /// <summary>
    /// Returns a point on the perimeter of this circle that is closest to the specified point.
    /// </summary>
    public float2 ClosestPoint(float2 point)
    {
        float2 towards = point - Center;
        float length = math.length(towards);
        if (length < math.EPSILON)
            return point;

        // TODO: Performance check branch vs bursted max
        if (length < Radius)
            return point;

        return Center + Radius * (towards / length);
    }

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public float Distance(CircleShape circle) => ShapeUtility.DistanceCircleAndCircle(this, circle);

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public float Distance(RectangleShape rectangle) => ShapeUtility.DistanceRectangleAndCircle(rectangle, this);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(float2 point) => ShapeUtility.OverlapCircleAndPoint(this, point);

    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(LineShape line) => ShapeUtility.OverlapCircleAndLine(this, line);

    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(CircleShape circle) => ShapeUtility.OverlapCircleAndCircle(this, circle);

    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(RectangleShape rectangle) => ShapeUtility.OverlapRectangleAndCircle(rectangle, this);

    /// <summary>
    /// Returns minimum bounding circle that contains both circles.
    /// </summary>
    public static CircleShape Union(CircleShape a, CircleShape b)
    {
        return new CircleShape((a.Center + b.Center) * 0.5f,
            math.distance(a.Center, b.Center) * 0.5f + math.max(a.Radius, b.Radius));
    }
}


/// <summary>
/// A rectangle shape.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public partial struct RectangleShape : IEquatable<RectangleShape>
{
    /// <summary>
    /// The minimum point of rectangle.
    /// </summary>
    public float2 Position;

    /// <summary>
    /// The size of the rectangle.
    /// </summary>
    public float2 Size;

    /// <summary>
    /// The half size of the rectangle.
    /// </summary>
    public float2 Extent
    {
        get => Size * 0.5f;
        set => Size = value * 2;
    }

    /// <summary>
    /// The center of the rectangle.
    /// </summary>
    public float2 Center
    {
        get => Position + Extent;
        set => Position = value - Extent;
    }

    /// <summary>
    /// The minimum point of the rectangle.
    /// </summary>
    public float2 Min
    {
        get => Position;
        set => Position = value;
    }

    /// <summary>
    /// The maximum point of the rectangle.
    /// </summary>
    public float2 Max
    {
        get => Position + Size;
        set => Position = value - Size;
    }

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public float Width
    {
        get => Size.x;
        set => Size.x = value;
    }

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public float Height
    {
        get => Size.y;
        set => Size.y = value;
    }

    /// <summary>
    /// Returns perimeter of the rectangle.
    /// </summary>
    public float Perimeter => 2f * (Size.x + Size.y);

    /// <summary>
    /// Returns area of the rectangle.
    /// </summary>
    public float Area => Size.x * Size.y;

    public RectangleShape(float2 position, float2 size)
    {
        Position = position;
        Size = size;
    }

    /// <inheritdoc />
    public bool Equals(RectangleShape other) => math.all(Position == other.Position & Size == other.Size);

    /// <inheritdoc />
    public override bool Equals(object other) => throw new NotImplementedException();

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc />
    public static bool operator ==(RectangleShape lhs, RectangleShape rhs) =>
        math.all(lhs.Position == rhs.Position & lhs.Size == rhs.Size);

    /// <inheritdoc />
    public static bool operator !=(RectangleShape lhs, RectangleShape rhs) => !(lhs == rhs);

    /// <summary>
    /// Returns a point on the perimeter of this rectangle that is closest to the specified point.
    /// </summary>
    public float2 ClosestPoint(float2 point) => math.clamp(point, Min, Max);

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public float Distance(CircleShape circle) => ShapeUtility.DistanceRectangleAndCircle(this, circle);

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public float Distance(RectangleShape rectangle) => ShapeUtility.DistanceRectangleAndRectangle(this, rectangle);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(float2 point) => ShapeUtility.OverlapRectangleAndPoint(this, point);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(LineShape line) => ShapeUtility.OverlapRectangleAndLine(this, line);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(CircleShape circle) => ShapeUtility.OverlapRectangleAndCircle(this, circle);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(RectangleShape rectangle) => ShapeUtility.OverlapRectangleAndRectangle(this, rectangle);


    /// <summary>
    /// Returns rectangle lines.
    /// </summary>
    public void GetLines(out LineShape a, out LineShape b, out LineShape c, out LineShape d) =>
        ShapeUtility.RectangleLines(this, out a, out b, out c, out d);

    /// <summary>
    /// Returns rectangle points in clockwise order. First point is rectangle position.
    /// </summary>
    public void GetPoints(out float2 a, out float2 b, out float2 c, out float2 d)
    {
        var min = Min;
        var max = Max;
        a = new float2(min.x, min.y);
        b = new float2(min.x, max.y);
        c = new float2(max.x, max.y);
        d = new float2(max.x, min.y);
    }

    /// <summary>
    /// Returns minimum bounding rectangle that contains both rectangles.
    /// </summary>
    public static RectangleShape Union(RectangleShape a, RectangleShape b)
    {
        float2 min = math.min(a.Min, b.Min);
        float2 max = math.max(a.Max, b.Max);
        return new RectangleShape(min, max - min);
    }
}

/// <summary>
/// LineShape segment that has start and end points.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct LineShape : IEquatable<LineShape>
{
    /// <summary>
    /// LineShape start position.
    /// </summary>
    public float2 From;

    /// <summary>
    /// LineShape end position.
    /// </summary>
    public float2 To;

    /// <summary>
    /// LineShape vector.
    /// </summary>
    public float2 Towards => To - From;

    /// <summary>
    /// Returns direction of the line.
    /// </summary>
    public float2 Direction => math.normalizesafe(Towards);

    /// <summary>
    /// Mid point of the line.
    /// </summary>
    public float2 MidPoint => (To + From) * 0.5f;

    /// <summary>
    /// Returns length of the line.
    /// </summary>
    public float Length => math.distance(To, From);

    public LineShape(float2 from, float2 to)
    {
        From = from;
        To = to;
    }

    /// <inheritdoc />
    public bool Equals(LineShape other) => math.all(From == other.From & To == other.To);

    /// <inheritdoc />
    public override bool Equals(object other) => throw new NotImplementedException();

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc />
    public static bool operator ==(LineShape lhs, LineShape rhs) => math.all(lhs.From == rhs.From & lhs.To == rhs.To);

    /// <inheritdoc />
    public static bool operator !=(LineShape lhs, LineShape rhs) => !(lhs == rhs);

    /// <inheritdoc />
    public static implicit operator LineShape(float value) => new LineShape(value, value);

    /// <summary>
    /// Returns a point on the perimeter of this rectangle that is closest to the specified point.
    /// </summary>
    public float2 ClosestPoint(float2 point)
    {
        float2 towards = Towards;

        float lengthSq = math.lengthsq(towards);
        if (lengthSq < math.EPSILON)
            return point;

        float t = math.dot(point - From, towards) / lengthSq;

        // Force within the segment
        t = math.saturate(t);

        return From + t * towards;
    }

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public float Distance(float2 point) => math.distance(ClosestPoint(point), point);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(LineShape line) => ShapeUtility.OverlapLineAndLine(this, line);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(CircleShape circle) => ShapeUtility.OverlapCircleAndLine(circle, this);

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public bool Overlap(RectangleShape rectangle) => ShapeUtility.OverlapRectangleAndLine(rectangle, this);


    /// <summary>
    /// Converts to ray.
    /// </summary>
    public Ray ToRay() => new Ray(From, Towards);
}

/// <summary>
/// Parametric line that is specified by center and direction.
/// </summary>
[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Ray
{
    /// <summary>
    /// LineShape center point.
    /// </summary>
    public float2 Origin;

    /// <summary>
    /// LineShape direction vector.
    /// </summary>
    public float2 Direction;

    public Ray(float2 origin, float2 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    /// <summary>
    /// Returns point along the ray at given time.
    /// </summary>
    public float2 GetPoint(float t)
    {
        return Origin + Direction * t;
    }

    /// <summary>
    /// Returns a point on the perimeter of this rectangle that is closest to the specified point.
    /// </summary>
    public static float2 ClosestPoint(in Ray ray, float2 point)
    {
        float2 towards = ray.Direction;

        float lengthSq = math.lengthsq(towards);
        if (lengthSq < math.EPSILON)
            return point;

        float t = math.dot(point - ray.Origin, towards) / lengthSq;

        return ray.GetPoint(t);
    }

    /// <summary>
    /// Returns intersection point between two rays.
    /// </summary>
    public static bool IntersectionPoint(Ray a, Ray b, out float2 point)
    {
        if (ShapeUtility.Intersection(a, b, out float2 t))
        {
            point = a.GetPoint(t.x);
            return true;
        }

        point = 0;
        return false;
    }

    /// <summary>
    /// Returns intersection time between ray and shape.
    /// </summary>
    public bool Intersection(LineShape line, out float t) => ShapeUtility.Intersection(this, line, out t);

    /// <summary>
    /// Returns intersection times between ray and shape.
    /// </summary>
    public bool Intersection(CircleShape circle, out float2 t) => ShapeUtility.Intersection(this, circle, out t);

    /// <summary>
    /// Returns intersection times between ray and shape.
    /// </summary>
    public bool Intersection(RectangleShape rectangle, out float2 t) =>
        ShapeUtility.Intersection(this, rectangle, out t);

    /// <summary>
    /// Returns intersection line between ray and shape.
    /// </summary>
    public bool IntersectionLine(CircleShape circle, out LineShape intersection) =>
        ShapeUtility.Intersection(this, circle, out intersection);

    /// <summary>
    /// Returns intersection line between ray and shape.
    /// </summary>
    public bool IntersectionLine(RectangleShape rectangle, out LineShape intersection) =>
        ShapeUtility.Intersection(this, rectangle, out intersection);

    /// <summary>
    /// Returns line at specific time range.
    /// </summary>
    public LineShape ToLine(float2 t) => new LineShape(GetPoint(t.x), GetPoint(t.y));
}