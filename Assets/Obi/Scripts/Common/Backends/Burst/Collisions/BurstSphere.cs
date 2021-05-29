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
    public struct BurstSphere : BurstLocalOptimization.IDistanceFunction, IBurstCollider
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

            float radius = shape.size.x * math.cmax(transform.scale.xyz);
            float distanceToCenter = math.length(point);

            float4 normal = point / (distanceToCenter + BurstMath.epsilon);

            projectedPoint.point = transform.TransformPointUnscaled(center + normal * (radius + shape.contactOffset));
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

            var colliderPoint = BurstLocalOptimization.Optimize<BurstSphere>(ref this, positions, radii, simplices, simplexStart, simplexSize,
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
                float4 center = shape.center * transform.scale;
                position = transform.InverseTransformPointUnscaled(position) - center;

                float radius = shape.size.x * math.cmax(transform.scale.xyz);
                float distanceToCenter = math.length(position);

                float4 normal = position / distanceToCenter;

                BurstContact c = new BurstContact
                {
                    entityA = particleIndex,
                    entityB = colliderIndex,
                    point = center + normal * radius,
                    normal = normal,
                };

                c.point = transform.TransformPointUnscaled(c.point);
                c.normal = transform.TransformDirection(c.normal);

                c.distance = distanceToCenter - radius - (shape.contactOffset + BurstMath.EllipsoidRadius(c.normal, orientation, radii.xyz));

                contacts.Enqueue(c);
            }*/
        }

}
#endif