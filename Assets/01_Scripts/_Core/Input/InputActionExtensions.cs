using R3;
using UnityEngine.InputSystem;

namespace RottenNoble.Core.Input
{
    /// <summary>
    /// UnityEngine.InputSystem.InputAction 에 R3 Observable 확장 메서드를 추가합니다.
    /// </summary>
    public static class InputActionExtensions
    {
        /// <summary>performed 이벤트를 값(T)으로 변환한 Observable 반환</summary>
        public static Observable<T> PerformedValueAsObservable<T>(this InputAction action)
            where T : struct
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.performed += h,
                    h => action.performed -= h)
                .Select(ctx => ctx.ReadValue<T>());
        }

        /// <summary>started 이벤트를 Observable로 반환</summary>
        public static Observable<InputAction.CallbackContext> StartedAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.started += h,
                    h => action.started -= h);
        }

        /// <summary>performed 이벤트를 Observable로 반환</summary>
        public static Observable<InputAction.CallbackContext> PerformedAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.performed += h,
                    h => action.performed -= h);
        }

        /// <summary>canceled 이벤트를 Observable로 반환</summary>
        public static Observable<InputAction.CallbackContext> CanceledAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.canceled += h,
                    h => action.canceled -= h);
        }

        /// <summary>
        /// performed → true, canceled → false 로 매핑한 bool Observable 반환.
        /// 버튼 눌림/뗌 상태 추적에 사용합니다.
        /// </summary>
        public static Observable<bool> AsBoolObservable(this InputAction action)
        {
            var performed = action.PerformedAsObservable().Select(_ => true);
            var canceled  = action.CanceledAsObservable().Select(_ => false);
            return performed.Merge(canceled);
        }
    }
}
