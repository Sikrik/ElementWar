using UnityEngine;

namespace Player
{
    public class PlayerWeapon: MonoBehaviour
    {
        [Tooltip("子弹的生成位置")]
        public Transform bulletSpawnPoint;
        [Tooltip("子弹的预制体")]
        public PlayerWeaponBullet bulletEffectPrefab;
        [Tooltip("子弹火花的预制体")]
        public GameObject bulletSparkPrefab;
        [Tooltip("子弹的发射频率")]
        public float bulletInterval = 0.15f;

        private float _lastFireTime;//上一次子弹发射的时间

        public void Fire(Vector3 targetPos)
        {
            //检查发射间隔
            if (Time.time - _lastFireTime < bulletInterval)
                return;
            _lastFireTime = Time.time;
            //计算发射方向
            Vector3 direction = targetPos - bulletSpawnPoint.position;
            direction.Normalize();
            //实例化子弹预制体
            PlayerWeaponBullet bulletEffect = Instantiate(bulletEffectPrefab, bulletSpawnPoint.position, Quaternion.identity);
            //实例化火花预制体
            GameObject spark = Instantiate(bulletSparkPrefab, bulletSpawnPoint.position, Quaternion.identity);
            spark.transform.forward = direction;
            //设置子弹朝向
            bulletEffect.transform.forward = direction;
        }
    }
}