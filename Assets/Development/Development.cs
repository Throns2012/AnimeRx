﻿using System;
using System.Collections;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AnimeRx.Dev
{
    public class Development : MonoBehaviour
    {
        [SerializeField] private GameObject cube = default;
        [SerializeField] private GameObject cube2 = default;
        [SerializeField] private GameObject cube3 = default;
        [SerializeField] private GameObject sphere = default;
        [SerializeField] private GameObject sphere2 = default;
        [SerializeField] private AnimationCurve curve = default;
        [SerializeField] private Slider slider1 = default;
        [SerializeField] private Slider slider2 = default;

        public IEnumerator Start()
        {
            cube.transform.position = new Vector3(-5f, -1f, 0f);
            cube2.transform.position = new Vector3(-5f, 1f, 0f);
            cube3.transform.position = new Vector3(0f, 3f, 0f);

            // cube.SetActive(false);
            // cube2.SetActive(false);
            cube3.SetActive(false);
            sphere.SetActive(false);
            sphere2.SetActive(false);

            slider1.gameObject.SetActive(false);
            slider2.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            Sample2();
            yield return null;
        }

        private void BugCheck()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), Motion.Uniform(4f))
                .PlayRelative(new Vector3(0f, 3f, 0f), Motion.Uniform(4f))
                .SubscribeToPosition(cube);
        }

        private void Sample1()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), Motion.Uniform(4f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample2()
        {
            var animator = Motion.Uniform(5f);
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), animator)
                .Play(new Vector3(0f, 3f, 0f), animator)
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample3()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), Easing.OutQuad(2f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample4()
        {
            var positions = new[]
            {
                new Vector3(-5f, 0f, 0f),
                new Vector3(0f, 3f, 0f),
                new Vector3(5f, 0f, 0f),
                new Vector3(0f, -3f, 0f),
                new Vector3(-5f, 0f, 0f),
            };

            Anime.Play(positions, Easing.InOutSine(6f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample5()
        {
            var x = Anime.Play(-5f, 5f, Easing.InOutSine(3f));

            var y = Anime.Play(0f, 3f, Easing.InOutSine(1.5f))
                .Play(0f, Easing.InOutSine(1.5f));

            var z = Anime.Stay(0f);

            Observable.CombineLatest(x, y, z)
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample6()
        {
            cube.transform.position
                .Play(new Vector3(3f, 3f, 0f), Easing.OutBack(2f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample7()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f),
                    Easing.InOutSine(1f))
                .Play(new Vector3(-5f, 0f, 0f), Easing.InOutSine(1f))
                .Repeat()
                .SubscribeToPosition(cube);
        }

        private void Sample8()
        {
            Anime.Play(0f, Mathf.PI * 2f, Easing.OutCubic(3f))
                .Select(x => new Vector3(Mathf.Sin(x), Mathf.Cos(x), 0.0f))
                .Select(x => x * 3f)
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample9()
        {
            var leftCube1 = Anime
                .Play(new Vector3(-5f, 0f, 0f), new Vector3(-0.5f, 0f, 0f), Easing.Linear(2.5f))
                .DoToPosition(cube);

            var rightCube1 = Anime
                .Play(new Vector3(5f, 0f, 0f), new Vector3(0.5f, 0f, 0f), Easing.OutCubic(1f))
                .DoToPosition(cube2);

            var leftCube2 = Anime
                .Play(new Vector3(-0.5f, 0f, 0f), new Vector3(-0.5f, 3f, 0f),
                    Easing.OutCubic(1f))
                .DoToPosition(cube);

            var rightCube2 = Anime
                .Play(new Vector3(0.5f, 0f, 0f), new Vector3(0.5f, 3f, 0f),
                    Easing.OutCubic(1f))
                .DoToPosition(cube2);

            Observable.WhenAll(leftCube1, rightCube1)
                .ContinueWith(Observable.WhenAll(leftCube2, rightCube2))
                .StopRecording()
                .Subscribe();
        }

        private void Sample10()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(0f, 0f, 0f), Easing.OutExpo(2f))
                .Sleep(1f)
                .Play(new Vector3(5f, 0f, 0f), Easing.OutExpo(2f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample11()
        {
            Anime.Play(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), Motion.From(curve, 3f))
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private IEnumerator Sample12()
        {
            var hp = new ReactiveProperty<int>(100);
            var gauge = new ReactiveProperty<float>(100.0f);

            // HPゲージは、実際の値に1.5秒かけて追いつく
            hp
                .Select(x => Anime.Play(gauge.Value, x, Easing.OutSine(1.5f)))
                .Switch()
                .Subscribe(x => gauge.Value = x);

            gauge.Subscribe(x =>
            {
                // HPゲージの長さにする
                Debug.LogFormat("hp: {0}", x);
            });

            yield return new WaitForSeconds(1.0f);

            Debug.Log("ダメージを受けてHPが30に！");
            hp.Value = 30;

            yield return new WaitForSeconds(1.0f);

            Debug.Log("回復してHPが80に！");
            hp.Value = 80;
        }

        private void Sample13()
        {
            var hp = new ReactiveProperty<float>(1.0f);
            var gauge = new ReactiveProperty<float>(1.0f);

            slider1.OnValueChangedAsObservable().Subscribe(x => hp.Value = x);

            hp
                .Select(x => Anime.Play(gauge.Value, x, Easing.OutCubic(1f)))
                .Switch()
                .Subscribe(x => gauge.Value = x);

            gauge.Subscribe(x => { slider2.value = x; });

            Anime.Sleep(0f)
                .DoOnCompleted(() => slider1.value = 0.3f)
                .Sleep(1f)
                .DoOnCompleted(() => slider1.value = 0.8f)
                .Sleep(1f)
                .DoOnCompleted(() => slider1.value = 0.0f)
                .Sleep(0.5f)
                .DoOnCompleted(() => slider1.value = 1.0f)
                .Subscribe();
        }

        private void Sample14()
        {
            Anime.PlayRelative(new Vector3(-5f, 0.75f, 0f), new Vector3(5f, 0f, 0f),
                    Easing.InCubic(Velocity.FromPerSecond(2f)))
                .PlayRelative(new Vector3(5f, 0f, 0f), Easing.OutCubic(Velocity.FromPerSecond(2f)))
                .SubscribeToPosition(cube);

            Anime.PlayRelative(new Vector3(-5f, -0.75f, 0f), new Vector3(5f, 0f, 0f),
                    Easing.InCubic(Velocity.FromPerSecond(2f)))
                .PlayRelative(new Vector3(5f, 0f, 0f), Easing.OutCubic(Velocity.FromPerSecond(2f)))
                .SubscribeToPosition(cube2);
        }

        private void Sample15()
        {
            var circle = Anime.Play(0f, Mathf.PI * 2f * 6f, Easing.Linear(6f))
                .Select(x => new Vector3(Mathf.Sin(x), Mathf.Cos(x), 0.0f));

            var radius = Anime.Play(3f, 0f, Easing.InOutSine(3f))
                .Play(3f, Easing.InOutSine(3f));

            Observable.CombineLatest(
                    circle,
                    radius,
                    Tuple.Create
                )
                .Select(x => x.Item1 * x.Item2)
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        private void Sample16()
        {
            Anime.PlayRelative(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f),
                    Easing.Linear(1f))
                .PlayRelative(new Vector3(5f, 0f, 0f), Easing.Linear(1f))
                .Do(x => Debug.LogFormat("cube1 {0} {1}", Time.time, x.x))
                .SubscribeToPosition(cube);

            Anime.PlayRelative(new Vector3(-5f, -1f, 0f), new Vector3(5f, 0f, 0f),
                    Easing.Linear(1f))
                .PlayRelative(new Vector3(5f, 0f, 0f), Easing.Linear(1f))
                .Do(x => Debug.LogFormat("cube2 {0} {1}", Time.time, x.x))
                .SubscribeToPosition(cube2);

            Observable.Interval(TimeSpan.FromSeconds(5f))
                .Subscribe(_ => Sample16());
        }

        private void Sample17()
        {
            var flow = Anime.Play(Easing.InOutExpo(2.5f))
                .Sleep(0.5f)
                .Play(1.0f, 0.0f, Easing.InOutExpo(2.5f));

            flow
                .Lerp(new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f))
                .SubscribeToPosition(cube);

            flow
                .Range(0.0f, 0.5f)
                .Lerp(new Vector3(-5f, -1f, 0f), new Vector3(0f, -1f, 0f))
                .SubscribeToPosition(cube2);
        }

        private void Sample18()
        {
            var circle = Anime
                .Play(0f, Mathf.PI * 2f, Easing.OutCubic(3f))
                .Select(x => new Vector3(Mathf.Sin(x), Mathf.Cos(x), 0.0f))
                .Select(x => x * 3f);

            circle
                .SubscribeToPosition(cube);

            circle
                .Delay(TimeSpan.FromSeconds(0.3f))
                .SubscribeToPosition(cube2);

            circle
                .Delay(TimeSpan.FromSeconds(0.55f))
                .SubscribeToPosition(cube3);
        }

        private void Sample19()
        {
            Anime.Play(new Vector3(0f, 0f, 0f), new Vector3(3f, 0f, 0f), new Sample19Animator())
                .PlayRelative(new Vector3(0f, 3f, 0f), Easing.Linear(1f))
                .SubscribeToPosition(cube);
        }

        public class Sample19Animator : IAnimator
        {
            public float CalcFinishTime(float distance)
            {
                return 3.0f;
            }

            public float CalcPosition(float time, float distance)
            {
                return 0.0f;
            }
        }

        public void Sample20()
        {
            var circle = Anime
                .Play(Mathf.PI, Mathf.PI * 2f * 3f, Easing.InOutSine(3f))
                .Select(x => new Vector3(Mathf.Sin(x), Mathf.Cos(x), 0f));

            var straight = Anime
                .Play(-3f, 3f, Easing.InOutSine(3f))
                .Select(x => new Vector3(0f, x, 0f));

            Observable.CombineLatest(circle, straight)
                .Sum()
                .StopRecording()
                .SubscribeToPosition(cube);
        }

        public void Sample21()
        {
            var x = Anime.Play(1.0f, 0.5f, Easing.InOutSine(1f))
                .Play(1.0f, Easing.OutElastic(0.8f));

            var y = Anime.Play(1.0f, 1.2f, Easing.InOutSine(1f))
                .Play(1.0f, Easing.OutElastic(0.8f));

            Observable.CombineLatest(x, y)
                .Select(s => new Vector3(s[0], s[1], 1f))
                .SubscribeToLocalScale(sphere);
        }

        public void Sample22()
        {
            Anime.Play(7.5f, 3f, Easing.OutElastic(1f))
                .SubscribeToPositionY(sphere2);
        }

        public void Sample23()
        {
            /*
            Anime.Shake(new Vector3(3f, 3f, 0f), Shaker.Simple(1f))ssss
                .SubscribeToPosition(sphere2);
            */
        }

        public void Sample24()
        {
            Anime.PlayInOut(-5f, -2f, 2f, 5f, Easing.InCubic(1f), Easing.OutCubic(1f))
                .StopRecording()
                .SubscribeToPositionX(cube);
        }

        public void Sample25()
        {
            Anime.PlayInOut(-5f, -2f, 2f, 5f, Easing.InCubic(1.5f), Easing.OutCubic(1.5f))
                .StopRecording()
                .DoOnCompleted(() => Debug.LogFormat("{0} Complete", Time.time))
                .SubscribeToPositionX(cube);

            Anime.PlayInOut(-5f, -2f, 0f, 3f, Easing.InCubic(1.5f), Easing.OutCubic(1.5f))
                .DoOnCompleted(() => Debug.LogFormat("{0} Complete", Time.time))
                .SubscribeToPositionX(cube2);
        }

        public void Sample26()
        {
            Anime.PlayInOut(-5f, -2f, 2f, 5f, Easing.InCubic(1f), Easing.OutCubic(5f))
                .StopRecording()
                .SubscribeToPositionX(cube);
        }

        public void Update()
        {
            // Debug.LogFormat("update {0} {1} {2}", Time.time, cube.transform.position.x, cube2.transform.position.x);
        }
    }

    public static class Util
    {
        public static IObservable<T> StopRecording<T>(this IObservable<T> source)
        {
            return source.DoOnCompleted(() =>
            {
                Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => EditorApplication.isPlaying = false);
            });
        }

        public static IObservable<T> StopRecordingSoon<T>(this IObservable<T> source)
        {
            return source.DoOnCompleted(() =>
            {
                EditorApplication.isPlaying = false;
            });
        }
    }
}
