using UnityEngine;

namespace Mercury
{
    public class SkillRaceterMoonAttack : MonoBehaviour, ISkill
    {
        [SerializeField] [Header("拖拽赋值，最好不要动")]
        private SkillGeneralAttack _atk1 = null;

        [SerializeField] [Header("拖拽赋值，最好不要动")]
        private SkillGeneralAttack _atk2 = null;

        [SerializeField] [Header("拖拽赋值，最好不要动")]
        private SkillMultiAttack _atk3 = null;

        [SerializeField] [Header("当前使用的第n段平A,仅用于debug")]
        private MonoBehaviour _nowSkill;

        private ISkillOwner _skillUser;

        /// <summary>
        /// -1:n
        /// 0:per
        /// 1:u
        /// 2:post
        /// </summary>
        private int _nowSkillState;

        private bool _isNowStateDone;
        private float _nowStatePreTime;
        private float _nowStatePostTime;

        /// <summary>
        /// 施放技能用的key，改键时需一起修改
        /// </summary>
        [Header("施放技能用的key")] public KeyCode skillKey = KeyCode.A;

        public bool isDone;
        public float postUseTime;

        public AssetLocation Id { get; private set; }
        float ISkill.PerUseTime => 0;
        float ISkill.PostUseTime => postUseTime;

        bool ISkill.IsDone => isDone;

        public SkillRaceterMoonAttack Init(
            AssetLocation id,
            ISkillOwner skillUser,
            IAttackable atker,
            KeyCode code,
            GameObject user,
            GameObject effectPrefab)
        {
            Id = id;
            _skillUser = skillUser;
            skillKey = code;
            _atk1.GetComponent<SkillGeneralAttack>()
                .Init(Const.RaceterMoonAtk1, skillUser, atker, user, effectPrefab);
            _atk2.GetComponent<SkillGeneralAttack>()
                .Init(Const.RaceterMoonAtk2, skillUser, atker, user, effectPrefab);
            _atk3.GetComponent<SkillMultiAttack>()
                .Init(Const.RaceterMoonAtk3, skillUser, atker, user, effectPrefab);
            return this;
        }

        public bool CanUse()
        {
            //暂时不允许打断其他技能
            return _skillUser.SkillSystem.NowState == SkillState.Normal;
        }

        public void OnPreUse()
        {
            _nowSkill = _atk1; //下一个使用的技能是第一段平A
            _nowSkillState = -1; //普通状态
            isDone = false;
        }

        public void OnUsing()
        {
            var now = Time.time;
            var nowSkill = (ISkill) _nowSkill;
            switch (_nowSkillState) //手动状态机，有5种状态
            {
                case -1: //普通
                    _nowStatePreTime = Time.time + nowSkill.PerUseTime; //设置前摇
                    _nowSkillState = 0; //前摇状态
                    break;
                case 2:
                {
                    if (_nowStatePostTime <= now) //当前平A后摇结束了,都没有新输入
                    {
                        isDone = true; //技能完成
                    }

                    if (Input.GetKeyDown(skillKey)) //有新输入
                    {
                        _nowSkillState = 0; //前摇
                        _nowSkill = NextSkill();
                        if (_nowSkill == null) //后续没有技能了
                        {
                            isDone = true; //技能完成
                            postUseTime = nowSkill.PostUseTime; //设置后摇
                        }

                        _nowStatePreTime = Time.time + nowSkill.PerUseTime;
                    }

                    return;
                }
                case 0 when !(_nowStatePreTime <= now): //前摇还没结束
                    return;
                case 0: //前摇结束了
                    nowSkill.OnPreUse();
                    _nowSkillState = 1;
                    break;
                case 1: //使用中
                {
                    nowSkill.OnUsing();
                    _isNowStateDone = nowSkill.IsDone; //使用完成了吗
                    if (!_isNowStateDone)
                    {
                        return;
                    }

                    _nowSkillState = 2; //后摇
                    _nowStatePostTime = now + nowSkill.PostUseTime; //后摇时间
                    nowSkill.OnPostUse();
                    break;
                }
                default: //小老弟你怎么回事
                    isDone = true;
                    break;
            }
        }

        public void OnPostUse()
        {
            _nowSkillState = -1;
            _nowSkill = null;
            isDone = false;
            postUseTime = 0;
        }

        private MonoBehaviour NextSkill() //手动设置下一个技能
        {
            if (_nowSkill == _atk1)
            {
                return _atk2;
            }

            return _nowSkill == _atk2 ? _atk3 : null;
        }
    }
}