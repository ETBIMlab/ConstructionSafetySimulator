#if (OBI_BURST && OBI_MATHEMATICS && OBI_COLLECTIONS)
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

namespace Obi
{
    public struct BurstCapsule : BurstLocalOptimization.IDistanceFunction, IBurstCollider
    {
        public BurstColliderShape shape;
        public BurstAffineTransform transform;
        public float dt;

        public void Evaluate(float4 point, ref BurstLocalOptimization.SurfacePoint projectedPoint)
        {
            float4 center = shape.center * transform.scale;
            point = transform.InverseTransformPointUnscaled(point) - center;

            if (shape.is2D != 0)
                point[2] = 0;

            int direction = (int)shape.size.z;
            float radius = shape.size.x * math.max(transform.scale[(direction + 1) % 3],
                                                   transform.scale[(direction + 2) % 3]);

            float height = math.max(radius, shape.size.y * 0.5f * transform.scale[direction]);
            float4 halfVector = float4.zero;
            halfVector[direction] = height - radius;

            float4 centerLine = BurstMath.NearestPointOnEdge(-halfVector, halfVector, point, out float mu);
            float4 centerToPoint = point - centerLine;
            float distanceToCenter = math.length(centerToPoint);

            float4 normal = centerToPoint / (distanceToCenter + BurstMath.epsilon);

            projectedPoint.point = transform.TransformPointUnscaled(center + centerLine + normal * (radius + shape.contactOffset));
            projectedPoint.normal = transform.TransformDirection(normal);
        }

        public void Contacts(int colliderIndex,

                              NativeArray<float4> positions,
                              NativeArray<float4> velocities,
                              NativeArray<float4> radii,

                              NativeArray<int> simplices,
                              in BurstAabb simplexBounds,
                              int simplexIndex,
                              int simplexStart,
                              int simplexSize,

                              NativeQueue<BurstContact>.ParallelWriter contacts,
                              int optimizationIterations,
                              float optimizationTolerance)
        {
            var co = new BurstContact() { bodyA = simplexIndex, bodyB = colliderIndex };
            float4 simplexBary = BurstMath.BarycenterForSimplexOfSize(simplexSize);

            var colliderPoint = BurstLocalOptimization.Optimize<BurstCapsule>(ref this, positions, radii, simplices, simplexStart, simplexSize,
                                                                              ref simplexBary, out float4 convexPoint, optimizationIterations, optimizationTolerance);

            co.pointB = colliderPoint.point;
            co.normal = colliderPoint.normal;
            co.pointA = simplexBary;

            contacts.Enqueue(co);
        }

        /*public static void Contacts(int particleIndex,
                                    float4 position,
                                    quaternion orientation,
                                    float4 radii,
                                    int colliderIndex,
                                    BurstAffineTransform transform,
                                    BurstColliderShape shape,
                                    NativeQueue<BurstContact>.ParallelWriter contacts)
        {
            BurstContact c = new BurstContact()
            {
                entityA = particleIndex,
                entityB = colliderIndex,
            };

            float4 center = shape.center * transform.scale;

            position = transform.InverseTransformPointUnscaled(position) - center;

            int direction = (int)shape.size.z;
            float radius = shape.size.x * math.max(transform.scale[(direction + 1) % 3], transform.scale[(direction + 2) % 3]);
            float height = math.max(radius, shape.size.y * 0.5f * transform.scale[direction]);
            float d = position[direction];
            float4 axisProj = float4.zero;
            float4 cap = float4.zero;

            axisProj[direction] = d;
            cap[direction] = height - radius;

            float4 centerToPoint;
            float centerToPointNorm;

            if (d > height - radius)
            { //one cap

                centerToPoint = position - cap;
                centerToPointNorm = math.length(centerToPoint);

                c.distance = centerToPointNorm - radius;
                c.normal = (centerToPoint / (centerToPointNorm + math.FLT_MIN_NORMAL));
                c.point = cap + c.normal * radius;

            }
            else if (d < -height + radius)
            { // other cap

                centerToPoint = position + cap;
                centerToPointNorm = math.length(centerToPoint);

                c.distance = centerToPointNorm - radius;
                c.normal = (centerToPoint / (centerToPointNorm + math.FLT_MIN_NORMAL));
                c.point = -cap + c.normal * radius;

            }
            else
            {//cylinder

                centerToPoint = position - axisProj;
                centerToPointNorm = math.length(centerToPoint);

                c.distance = centerToPointNorm - radius;
                c.normal = (centerToPoint / (centerToPointNorm + math.FLT_MIN_NORMAL));
                c.point = axisProj + c.normal * radius;

            }

            c.point += center;
            c.point = transform.TransformPointUnscaled(c.point);
            c.normal = transform.TransformDirection(c.normal);

            c.distance -= shape.contactOffset + BurstMath.EllipsoidRadius(c.normal, orientation, radii.xyz);

            contacts.Enqueue(c);
        }*/
    }

}
#endif