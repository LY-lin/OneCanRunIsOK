using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class EnemyDetectionModule : MonoBehaviour
    {
        [Tooltip("The point representing the source of target-detection raycasts for the enemy AI")]
        public Transform DetectionSourcePoint;

        [Tooltip("The max distance at which the enemy can see targets")]
        public float DetectionRange = 20f;

        [Tooltip("The max distance at which the enemy can attack its target")]
        public float AttackRange = 10f;

        [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
        public float KnownTargetTimeout = 4f;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;

        ActorsManager manager;

        public GameObject KnownDetectedTarget { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsSeeingTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }

        protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

        // Start is called before the first frame update
        void Start()
        {
            manager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyDetectionModule>(manager, this);
        }

        public virtual void HandleDetection(Actor self, Collider[] selfColliders)
        {
            float sqrDetectRange = DetectionRange * DetectionRange;
            IsSeeingTarget = false;
            float minDistance = Mathf.Infinity;
            // �������������н�ɫ
            foreach (Actor actor in manager.Actors)
            {
                // ���ڲ�ͬ��Ӫ
                if(actor.Affiliation != self.Affiliation)
                {
                    float sqrDistance = (actor.transform.position - DetectionSourcePoint.position).sqrMagnitude;
                    if (sqrDistance < sqrDetectRange && sqrDistance < minDistance)
                    {
                        // ��ȡ������һ����������ж���
                        RaycastHit[] hits = Physics.RaycastAll(DetectionSourcePoint.position,
                            (actor.AimPoint.position - DetectionSourcePoint.position).normalized, DetectionRange,
                            -1, QueryTriggerInteraction.Ignore);
                        // ����Ŀ��������Ķ����½����󱣴��Ŀ��
                        RaycastHit target = new RaycastHit();
                        // ��ʼ������������Զ
                        target.distance = Mathf.Infinity;
                        // ��һ���ҵõ�
                        bool found = false;
                        foreach (RaycastHit hit in hits)
                        {
                            // ���󲻰����Լ���������Ҿ����Сʱ������Ŀ�겢����found
                            if(!selfColliders.Contains(hit.collider) && hit.distance < target.distance)
                            {
                                target = hit;
                                found = true;
                            }
                        }

                        // �ҵ���
                        if (found)
                        {
                            // ��ȡ�����ɫ
                            Actor hitActor = target.collider.GetComponent<Actor>();
                            // ��ɫ��ͬһ��
                            if (hitActor == actor)
                            {
                                // ���ÿ���Ŀ��״̬�������µ�ǰ��̾���
                                IsSeeingTarget = true;
                                minDistance = sqrDistance;

                                // ��¼����Ŀ��������¼��Լ���⵽�Ķ���
                                TimeLastSeenTarget = Time.time;
                                KnownDetectedTarget = actor.AimPoint.gameObject;
                            }
                        }
                    }
                }
            }

            // �ж��Ƿ��ڹ�����Χ�ڲ�
            IsTargetInAttackRange = KnownDetectedTarget != null &&
                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= AttackRange;

            // �����֪��Ŀ�굫֪����⵽Ŀ�꣬��Ҫ������⵽�¼�
            if (!HadKnownTarget && KnownDetectedTarget != null)
            {
                OnDetect();
            }

            if (HadKnownTarget && KnownDetectedTarget == null)
            {
                OnLostTarget();
            }
        }

        // ��ʧĿ���¼�
        public virtual void OnLostTarget() => onLostTarget?.Invoke();

        // ��⵽���ʱ��
        public virtual void OnDetect() => onDetectedTarget?.Invoke();

        // �յ��˺��¼������Ӽ�ⷶΧ��
        public virtual void OnDamaged(GameObject damageSource)
        {
            TimeLastSeenTarget = Time.time;
            KnownDetectedTarget = damageSource;

        }

        // �����¼�
        public virtual void OnAttack()
        {

        }
    }
}
