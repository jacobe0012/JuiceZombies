using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public interface ITransformFloat2
{
    /// <summary>
    /// Returns transformed point.
    /// </summary>
    float2 Transform(float2 point);
}

public static partial class ShapeUtility
{
    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapCircleAndPoint(CircleShape circle, float2 point)
    {
        return distancesq(circle.Center, point) < circle.Radius * circle.Radius;
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapCircleAndCircle(CircleShape a, CircleShape b)
    {
        return distancesq(a.Center, b.Center) < (a.Radius + b.Radius).sq();
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapCircleAndLine(CircleShape b, LineShape a)
    {
        return distancesq(a.ClosestPoint(b.Center), b.Center) < b.Radius * b.Radius;
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapRectangleAndPoint(RectangleShape rectangle, float2 point)
    {
        return all(rectangle.Min < point & point < rectangle.Max);
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapRectangleAndRectangle(RectangleShape a, RectangleShape b)
    {
        return all((a.Max > b.Min) & (a.Min < b.Max));
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapRectangleAndCircle(RectangleShape rectangle, CircleShape circle)
    {
        return distancesq(rectangle.ClosestPoint(circle.Center), circle.Center) < circle.Radius * circle.Radius;
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapRectangleAndLine(RectangleShape b, LineShape a)
    {
        float2 d = a.Towards;

        float2 min = b.Min;
        float2 max = b.Max;

        float4 h = new float4(
            d.y * min.x - d.x * min.y,
            d.y * min.x - d.x * max.y,
            d.y * max.x - d.x * max.y,
            d.y * max.x - d.x * min.y);

        float det = math2.determinant(a.To, a.From);
        bool4 r = h + det > 0;

        // Are all of the same sign, then the segment definitely misses the box
        if (all(r) || all(!r))
            return false;

        // Check if ends overlap
        if (any(a.From >= max & a.To >= max) | any(a.From <= min & a.To <= min))
            return false;

        return true;
    }

    /// <summary>
    /// Returns true if shapes surfaces overlap.
    /// </summary>
    public static bool OverlapLineAndLine(LineShape a, LineShape b)
    {
        return Intersection(a.ToRay(), b.ToRay(), out float2 t) && all(t > 0 & t < 1);
    }


//    /// <summary>
//    /// Returns true if shapes surfaces overlap.
//    /// </summary>
//    [Obsolete("Obsolete from 1.3.0, use ConvexPolygonUtility.ContainsPoint")]
//    public static bool OverlapConvexPolygonAndPoint<T>(NativeSlice<float2> points, T tranform, float2 point2)
//where T : unmanaged, ITransformFloat2
//    {
//        return ConvexPolygonUtility.ContainsPoint(points, tranform, point2);
//    }

    public static bool Intersection(Ray ray, CircleShape circle, out float2 t)
    {
        float2 d = ray.Direction;
        float2 f = ray.Origin - circle.Center;

        // t2 * (d · d) + 2t*( f · d ) + ( f · f - r2 ) = 0

        // Check if has valid direction
        float a = dot(d, d);
        if (a < EPSILON)
        {
            t = 0;
            return false;
        }

        float b = 2 * dot(ray.Origin - circle.Center, d);
        float c = dot(f, f) - circle.Radius * circle.Radius;

        // We only care if discriminant is positive in that case there will be two intersections
        float discriminant = b * b - 4 * a * c;
        if (discriminant <= 0)
        {
            t = 0;
            return false;
        }

        discriminant = sqrt(discriminant);

        t = new float2(-b - discriminant, -b + discriminant) / (2 * a);

        return true;
    }

    public static bool Intersection(Ray ray, RectangleShape rectangle, out LineShape intersection)
    {
        if (Intersection(ray, rectangle, out float2 t))
        {
            intersection = ray.ToLine(t);
            return true;
        }

        intersection = 0;
        return false;
    }

    public static bool Intersection(Ray ray, RectangleShape rectangle, out float2 t)
    {
        RectangleLines(rectangle, out LineShape lineA, out LineShape lineB, out LineShape lineC, out LineShape lineD);

        if (Intersection(ray, lineA, out t.x))
        {
            if (Intersection(ray, lineB, out t.y))
                return true;
            if (Intersection(ray, lineC, out t.y))
                return true;
            if (Intersection(ray, lineD, out t.y))
                return true;
            return true;
        }

        if (Intersection(ray, lineB, out t.x))
        {
            if (Intersection(ray, lineC, out t.y))
                return true;
            if (Intersection(ray, lineD, out t.y))
                return true;
            return true;
        }

        if (Intersection(ray, lineC, out t.y))
        {
            if (Intersection(ray, lineD, out t.y))
                return true;
            return true;
        }

        t.y = 0;

        if (Intersection(ray, lineD, out t.x))
            return true;

        t.x = 0;

        return false;
    }

    public static bool Intersection(Ray ray, CircleShape circle, out LineShape intersection)
    {
        if (Intersection(ray, circle, out float2 t))
        {
            intersection = ray.ToLine(t);
            return true;
        }

        intersection = 0;
        return false;
    }

    public static bool Intersection(LineShape line, RectangleShape rectangle, out float2 pointA, out float2 pointB)
    {
        float2 position = rectangle.Position;
        float2 size = rectangle.Size;

        float2 a = position;
        float2 b = position + new float2(size.x, 0);
        float2 c = position + size;
        float2 d = position + new float2(0, size.y);

        LineShape lineA = new LineShape(a, b);
        LineShape lineB = new LineShape(b, c);
        LineShape lineC = new LineShape(c, d);
        LineShape lineD = new LineShape(d, a);

        if (Intersection(line, lineA, out pointA))
        {
            if (Intersection(line, lineB, out pointB))
                return true;
            if (Intersection(line, lineC, out pointB))
                return true;
            if (Intersection(line, lineD, out pointB))
                return true;
            return true;
        }

        if (Intersection(line, lineB, out pointA))
        {
            if (Intersection(line, lineC, out pointB))
                return true;
            if (Intersection(line, lineD, out pointB))
                return true;
            return true;
        }

        if (Intersection(line, lineC, out pointA))
        {
            if (Intersection(line, lineD, out pointB))
                return true;
            return true;
        }

        pointB = 0;

        if (Intersection(line, lineD, out pointA))
            return true;

        pointA = 0;

        return false;
    }

    public static bool IntersectionPolygonAndLine<T>(NativeSlice<float2> points, T tranform, LineShape line,
        out float2 point)
        where T : unmanaged, ITransformFloat2
    {
        for (int pointIndex = 0; pointIndex < points.Length; ++pointIndex)
        {
            float2 currentPoint = tranform.Transform(points[pointIndex]);
            float2 nextPoint = tranform.Transform(points[(pointIndex + 1) % points.Length]);

            var polygonLine = new LineShape(currentPoint, nextPoint);
            if (Intersection(polygonLine, line, out point))
                return true;
        }

        point = 0;
        return false;
    }

    /// <summary>
    /// Finds intersection times of two rays.
    /// Based on https://en.wikipedia.org/wiki/LineShape%E2%80%93line_intersection.
    /// </summary>
    public static bool Intersection(Ray a, Ray b, out float2 t)
    {
        float2 d = b.Origin - a.Origin;

        // Check if lines are not parallel
        float det = math2.determinant(a.Direction, b.Direction);
        if (abs(det) < math.EPSILON)
        {
            t = 0;
            return false;
        }

        t = new float2(math2.determinant(d, b.Direction), math2.determinant(d, a.Direction)) / det;
        return true;
    }

    public static bool Intersection(Ray a, LineShape b, out float t)
    {
        if (Intersection(a, b.ToRay(), out float2 u) && u.y >= 0 && u.y <= 1)
        {
            t = u.x;
            return true;
        }

        t = 0;
        return false;
    }

    public static bool Intersection(LineShape a, LineShape b, out float2 point)
    {
        Ray rayA = a.ToRay();
        if (Intersection(rayA, b.ToRay(), out float2 t) && all(t > 0 & t < 1))
        {
            point = rayA.GetPoint(t.x);
            return true;
        }

        point = 0;
        return false;
    }

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public static float DistanceRectangleAndRectangle(RectangleShape a, RectangleShape b)
    {
        float2 halfSizeA = a.Size * 0.5f;
        float2 halfSizeB = b.Size * 0.5f;
        float2 centerA = (a.Position + halfSizeA);
        float2 centerB = (b.Position + halfSizeB);
        float2 distance = abs(centerA - centerB);
        return length(max(distance - (halfSizeA + halfSizeB), 0));
    }

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public static float DistanceCircleAndCircle(CircleShape a, CircleShape b)
    {
        return max(distancesq(a.Center, b.Center) - (a.Radius + b.Radius).sq(), 0);
    }

    /// <summary>
    /// Returns minimum distance between shapes.
    /// </summary>
    public static float DistanceRectangleAndCircle(RectangleShape a, CircleShape b)
    {
        return distance(a.ClosestPoint(b.Center), b.Center);
    }

    /// <summary>
    /// Returns rectangle lines.
    /// </summary>
    public static void RectangleLines(RectangleShape rectangle, out LineShape a, out LineShape b, out LineShape c,
        out LineShape d)
    {
        float2 position = rectangle.Position;
        float2 size = rectangle.Size;

        float2 pointA = position;
        float2 pointB = position + new float2(size.x, 0);
        float2 pointC = position + size;
        float2 pointD = position + new float2(0, size.y);

        a = new LineShape(pointA, pointB);
        b = new LineShape(pointB, pointC);
        c = new LineShape(pointC, pointD);
        d = new LineShape(pointD, pointA);
    }
}

public static partial class math2
{
    public static float sum(this float2 value) => value.x + value.y;

    public static float sum(this float3 value) => value.x + value.y + value.z;

    public static float sum(this float4 value) => value.x + value.y + value.z + value.w;


    public static double sum(this double2 value) => value.x + value.y;

    public static double sum(this double3 value) => value.x + value.y + value.z;

    public static double sum(this double4 value) => value.x + value.y + value.z + value.w;


    public static int sum(this int2 value) => value.x + value.y;

    public static int sum(this int3 value) => value.x + value.y + value.z;

    public static int sum(this int4 value) => value.x + value.y + value.z + value.w;


    public static float sq(this float value) => value * value;

    public static double sq(this double value) => value * value;

    public static int sq(this int value) => value * value;


    public static float2 sort(float2 value) => value.x > value.y ? value.yx : value.xy;

    public static double2 sort(double2 value) => value.x > value.y ? value.yx : value.xy;

    public static int2 sort(int2 value) => value.x > value.y ? value.yx : value.xy;


    public static float3 sort(float3 value)
    {
        value.xy = sort(value.xy);
        if (value.z < value.x)
            return value.zxy;
        if (value.z < value.y)
            return value.xzy;
        return value.xyz;
    }

    public static double3 sort(double3 value)
    {
        value.xy = sort(value.xy);
        if (value.z < value.x)
            return value.zxy;
        if (value.z < value.y)
            return value.xzy;
        return value.xyz;
    }

    public static int3 sort(int3 value)
    {
        value.xy = sort(value.xy);
        if (value.z < value.x)
            return value.zxy;
        if (value.z < value.y)
            return value.xzy;
        return value.xyz;
    }


    public static float4 sort(float4 value)
    {
        value.xyz = sort(value.xyz);
        if (value.w < value.x)
            return value.wxyz;
        if (value.w < value.y)
            return value.xwyz;
        if (value.w < value.z)
            return value.xywz;
        return value.xyzw;
    }

    public static double4 sort(double4 value)
    {
        value.xyz = sort(value.xyz);
        if (value.w < value.x)
            return value.wxyz;
        if (value.w < value.y)
            return value.xwyz;
        if (value.w < value.z)
            return value.xywz;
        return value.xyzw;
    }


    public static int4 sort(int4 value)
    {
        value.xyz = sort(value.xyz);
        if (value.w < value.x)
            return value.wxyz;
        if (value.w < value.y)
            return value.xwyz;
        if (value.w < value.z)
            return value.xywz;
        return value.xyzw;
    }


    /// <summary>
    /// PI multiplied by two.
    /// </summary>
    public const float PI2 = 6.28318530718F;

    /// <summary>
    /// PI multiplied by two.
    /// </summary>
    public const double PI2_D = 6.2831853071795864769;

    /// <summary>
    /// Returns cross product of two vectors.
    /// </summary>
    public static float2 cross(float2 a, float2 b) => new float2(a.x * b.y, -a.y * b.x);

    /// <summary>
    /// Returns cross product of two vectors.
    /// </summary>
    public static double2 cross(double2 a, double2 b) => new double2(a.x * b.y, -a.y * b.x);

    /// <summary>
    /// Returns determinant of two vectors.
    /// Sum of cross product elements.
    /// </summary>
    public static float determinant(float2 a, float2 b) => a.x * b.y - a.y * b.x;

    /// <summary>
    /// Returns determinant of two vectors.
    /// Sum of cross product elements.
    /// </summary>
    public static double determinant(double2 a, double2 b) => a.x * b.y - a.y * b.x;

    /// <summary>
    /// Returns determinant of two vectors.
    /// Sum of cross product elements.
    /// </summary>
    public static float determinant(float3 a, float3 b)
    {
        return ((a.y * b.z) - (a.z * b.y)) - ((a.z * b.x) - (a.x * b.z)) + ((a.x * b.y) - (a.y * b.x));
    }

    /// <summary>
    /// Returns determinant of two vectors.
    /// Sum of cross product elements.
    /// </summary>
    public static double determinant(double3 a, double3 b)
    {
        return ((a.y * b.z) - (a.z * b.y)) - ((a.z * b.x) - (a.x * b.z)) + ((a.x * b.y) - (a.y * b.x));
    }

    /// <summary>
    /// Returns true if points ordered counter clockwise.
    /// </summary>
    public static bool iscclockwise(float2 a, float2 b, float2 c) => determinant(c - a, b - a) < 0;

    /// <summary>
    /// Returns true if points ordered counter clockwise.
    /// </summary>
    public static bool iscclockwise(float3 a, float3 b, float3 c) => determinant(c - a, b - a) < 0;

    /// <summary>
    /// Returns true if points ordered counter clockwise.
    /// </summary>
    public static bool iscclockwise(double2 a, double2 b, double2 c) => determinant(c - a, b - a) < 0;

    /// <summary>
    /// Returns true if points ordered counter clockwise.
    /// </summary>
    public static bool iscclockwise(double3 a, double3 b, double3 c) => determinant(c - a, b - a) < 0;

    /// <summary>
    /// Returns true if points ordered clockwise.
    /// </summary>
    public static bool isclockwise(float2 a, float2 b, float2 c) => determinant(c - a, b - a) > 0;

    /// <summary>
    /// Returns true if points ordered clockwise.
    /// </summary>
    public static bool isclockwise(float3 a, float3 b, float3 c) => determinant(c - a, b - a) > 0;

    /// <summary>
    /// Returns true if points ordered clockwise.
    /// </summary>
    public static bool isclockwise(double2 a, double2 b, double2 c) => determinant(c - a, b - a) > 0;

    /// <summary>
    /// Returns true if points ordered clockwise.
    /// </summary>
    public static bool isclockwise(double3 a, double3 b, double3 c) => determinant(c - a, b - a) > 0;


    /// <summary>
    /// Returns true if valid triangle exists knowing three edge lengths.
    /// </summary>
    public static bool istriangle(float a, float b, float c)
    {
        // Sum of two triangle edge is always lower than third
        return all(new bool3(
            a + b > c,
            a + c > b,
            b + c > a));
    }

    /// <summary>
    /// Returns true if valid triangle exists knowing three edge lengths.
    /// </summary>
    public static bool istriangle(double a, double b, double c)
    {
        // Sum of two triangle edge is always lower than third
        return all(new bool3(
            a + b > c,
            a + c > b,
            b + c > a));
    }

    /// <summary>
    /// Returns if quad meets the Delaunay condition. Where a, b, c forms clockwise sorted triangle.
    /// Based on https://en.wikipedia.org/wiki/Delaunay_triangulation.
    /// </summary>
    public static bool isdelaunay(float2 a, float2 b, float2 c, float2 d)
    {
        float2 ad = a - d;
        float2 bd = b - d;
        float2 cd = c - d;

        float2 d2 = d * d;

        float2 ad2 = a * a - d2;
        float2 bd2 = b * b - d2;
        float2 cd2 = c * c - d2;

        float determinant = math.determinant(new float3x3(
            new float3(ad.x, ad.y, ad2.x + ad2.y),
            new float3(bd.x, bd.y, bd2.x + bd2.y),
            new float3(cd.x, cd.y, cd2.x + cd2.y)
        ));

        return determinant >= 0;
    }

    /// <summary>
    /// Returns if quad meets the Delaunay condition. Where a, b, c forms clockwise sorted triangle.
    /// Based on https://en.wikipedia.org/wiki/Delaunay_triangulation.
    /// </summary>
    public static bool isdelaunay(double2 a, double2 b, double2 c, double2 d)
    {
        double2 ad = a - d;
        double2 bd = b - d;
        double2 cd = c - d;

        double2 d2 = d * d;

        double2 ad2 = a * a - d2;
        double2 bd2 = b * b - d2;
        double2 cd2 = c * c - d2;

        double determinant = math.determinant(new double3x3(
            new double3(ad.x, ad.y, ad2.x + ad2.y),
            new double3(bd.x, bd.y, bd2.x + bd2.y),
            new double3(cd.x, cd.y, cd2.x + cd2.y)
        ));

        return determinant >= 0;
    }

    /// <summary>
    /// Returns factorial of the value (etc 0! = 1, 1! = 1, 2! = 2, 3! = 6, 4! = 24 ...)
    /// Based on https://en.wikipedia.org/wiki/Factorial.
    /// </summary>
    public static int factorial(int value)
    {
        int factorial = 1;
        int count = value + 1;
        for (int i = 1; i < count; ++i)
            factorial *= i;
        return factorial;
    }

    /// <summary>
    /// Exchanges the values of a and b.
    /// </summary>
    public static void swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
}