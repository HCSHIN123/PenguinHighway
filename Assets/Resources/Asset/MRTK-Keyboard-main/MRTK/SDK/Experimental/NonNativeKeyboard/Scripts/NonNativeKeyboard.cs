using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    public class NonNativeKeyboard : MonoBehaviour
    {
        public static NonNativeKeyboard Instance { get; private set; }
        public enum LayoutType
        {
            Alpha
        }

        #region Callbacks
        public event EventHandler OnTextSubmitted = delegate { };
        public event Action<string> OnTextUpdated = delegate { };
        public event EventHandler OnClosed = delegate { };
        public event EventHandler OnPrevious = delegate { };
        public event EventHandler OnNext = delegate { };
        public event EventHandler OnPlacement = delegate { };
        #endregion Callbacks

        
        public TMP_InputField InputField = null;
        public AxisSlider InputFieldSlide = null;
        public bool SliderEnabled = true;
        public bool SubmitOnEnter = true;
        public Image AlphaKeyboard = null;
        private LayoutType m_LastKeyboardLayout = LayoutType.Alpha;

        [Header("Positioning")]
        [SerializeField]
        private float m_MaxScale = 1.0f;
        [SerializeField]
        private float m_MinScale = 1.0f;
        [SerializeField]
        private float m_MaxDistance = 3.5f;
        [SerializeField]
        private float m_MinDistance = 0.25f;

        public bool CloseOnInactivity = true;
        public float CloseOnInactivityTime = 15;
        private float _closingTime;

        public event Action<bool> OnKeyboardShifted = delegate { };
        public event Action<KeyboardValueKey> OnKeyboardValueKeyPressed = delegate { };
        public event Action<KeyboardKeyFunc> OnKeyboardFunctionKeyPressed = delegate { };
        private bool m_IsShifted = false;
        private bool m_IsCapslocked = false;

       
        public bool IsShifted
        {
            get { return m_IsShifted; }
        }

        public bool IsCapsLocked
        {
            get { return m_IsCapslocked; }
        }

        private int m_CaretPosition = 0;
        private Vector3 m_StartingScale = Vector3.one;
        private Vector3 m_ObjectBounds;
        private Color _defaultColor;
        private Image _recordImage;
        private AudioSource _audioSource;

        void Awake()
        {
            Instance = this;

            m_StartingScale = transform.localScale;
            Bounds canvasBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);

            RectTransform rect = GetComponent<RectTransform>();
            m_ObjectBounds = new Vector3(canvasBounds.size.x * rect.localScale.x, canvasBounds.size.y * rect.localScale.y, canvasBounds.size.z * rect.localScale.z);

            InputField.keyboardType = (TouchScreenKeyboardType)(int.MaxValue);

            gameObject.SetActive(false);
        }


        protected void Start()
        {
            InputField.onValueChanged.AddListener(DoTextUpdated);
        }

        private void DoTextUpdated(string value) => OnTextUpdated?.Invoke(value);

        private void LateUpdate()
        {
            if (SliderEnabled)
            {
                Vector3 nearPoint = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.forward);
                Vector3 relPos = transform.InverseTransformPoint(nearPoint);
                InputFieldSlide.TargetPoint = relPos;
            }
            CheckForCloseOnInactivityTimeExpired();
        }

        private void UpdateCaretPosition(int newPos) => InputField.caretPosition = newPos;

        protected void OnDisable()
        {            
            m_LastKeyboardLayout = LayoutType.Alpha;
            Clear();
        }

        void OnDestroy()
        {
            Instance = null;
        }

        #region Present Functions

        public void PresentKeyboard()
        {
            ResetClosingTime();
            gameObject.SetActive(true);
            ActivateSpecificKeyboard(LayoutType.Alpha);

            OnPlacement(this, EventArgs.Empty);

            InputField.ActivateInputField();
        }


        public void PresentKeyboard(string startText)
        {
            PresentKeyboard();
            Clear();
            InputField.text = startText;
        }

        
        public void PresentKeyboard(LayoutType keyboardType)
        {
            PresentKeyboard();
            ActivateSpecificKeyboard(keyboardType);
        }

        
        public void PresentKeyboard(string startText, LayoutType keyboardType)
        {
            PresentKeyboard(startText);
            ActivateSpecificKeyboard(keyboardType);
        }

        #endregion Present Functions
        
        public void RepositionKeyboard(Vector3 kbPos, float verticalOffset = 0.0f)
        {
            transform.position = kbPos;
            ScaleToSize();
            LookAtTargetOrigin();
        }

        
        public void RepositionKeyboard(Transform objectTransform, BoxCollider aCollider = null, float verticalOffset = 0.0f)
        {
            transform.position = objectTransform.position;

            if (aCollider != null)
            {
                float yTranslation = -((aCollider.bounds.size.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }
            else
            {
                float yTranslation = -((m_ObjectBounds.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }

            ScaleToSize();
            LookAtTargetOrigin();
        }

        private void ScaleToSize()
        {
            float distance = (transform.position - Camera.main.transform.position).magnitude;
            float distancePercent = (distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
            float scale = m_MinScale + (m_MaxScale - m_MinScale) * distancePercent;

            scale = Mathf.Clamp(scale, m_MinScale, m_MaxScale);
            transform.localScale = m_StartingScale * scale;

            Debug.LogFormat("Setting scale: {0} for distance: {1}", scale, distance);
        }

       
        private void LookAtTargetOrigin()
        {
            transform.LookAt(Camera.main.transform.position);
            transform.Rotate(Vector3.up, 180.0f);
        }

        
        private void ActivateSpecificKeyboard(LayoutType keyboardType)
        {
            DisableAllKeyboards();
            ResetKeyboardState();

            switch (keyboardType)
            {
                case LayoutType.Alpha:
                default:
                {
                    ShowAlphaKeyboard();
                    break;
                }
            }
        }

        #region Keyboard Functions

        #region Dictation
               
        private bool IsMicrophoneActive()
        {
            var result = _recordImage.color != _defaultColor;
            return result;
        }

        #endregion Dictation

        
        public void AppendValue(KeyboardValueKey valueKey)
        {
            IndicateActivity();
            string value = "";

            OnKeyboardValueKeyPressed(valueKey);

            if (m_IsShifted && !string.IsNullOrEmpty(valueKey.ShiftValue))
            {
                value = valueKey.ShiftValue;
            }
            else
            {
                value = valueKey.Value;
            }

            if (!m_IsCapslocked)
            {
                Shift(false);
            }

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, value);
            m_CaretPosition += value.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        public void FunctionKey(KeyboardKeyFunc functionKey)
        {
            IndicateActivity();
            OnKeyboardFunctionKeyPressed(functionKey);
            switch (functionKey.ButtonFunction)
            {
                case KeyboardKeyFunc.Function.Enter:
                {
                    Enter();
                    break;
                }

                case KeyboardKeyFunc.Function.Tab:
                {
                    Tab();
                    break;
                }

                case KeyboardKeyFunc.Function.ABC:
                {
                    ActivateSpecificKeyboard(m_LastKeyboardLayout);
                    break;
                }

                case KeyboardKeyFunc.Function.Previous:
                {
                    MoveCaretLeft();
                    break;
                }

                case KeyboardKeyFunc.Function.Next:
                {
                    MoveCaretRight();
                    break;
                }

                case KeyboardKeyFunc.Function.Close:
                {
                    Close();
                    break;
                }

                case KeyboardKeyFunc.Function.Shift:
                {
                    Shift(!m_IsShifted);
                    break;
                }

                case KeyboardKeyFunc.Function.CapsLock:
                {
                    CapsLock(!m_IsCapslocked);
                    break;
                }

                case KeyboardKeyFunc.Function.Backspace:
                {
                    Backspace();
                    break;
                }

                case KeyboardKeyFunc.Function.UNDEFINED:
                {
                    Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Backspace()
        {
            if (InputField.selectionFocusPosition != InputField.caretPosition || InputField.selectionAnchorPosition != InputField.caretPosition)
            {
                if (InputField.selectionAnchorPosition > InputField.selectionFocusPosition) // right to left
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionFocusPosition) + InputField.text.Substring(InputField.selectionAnchorPosition);
                    InputField.caretPosition = InputField.selectionFocusPosition;
                }
                else
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionAnchorPosition) + InputField.text.Substring(InputField.selectionFocusPosition);
                    InputField.caretPosition = InputField.selectionAnchorPosition;
                }

                m_CaretPosition = InputField.caretPosition;
                InputField.selectionAnchorPosition = m_CaretPosition;
                InputField.selectionFocusPosition = m_CaretPosition;
            }
            else
            {
                m_CaretPosition = InputField.caretPosition;

                if (m_CaretPosition > 0)
                {
                    --m_CaretPosition;
                    InputField.text = InputField.text.Remove(m_CaretPosition, 1);
                    UpdateCaretPosition(m_CaretPosition);
                }
            }
        }

        public void Previous()
        {
            OnPrevious(this, EventArgs.Empty);
        }

        public void Next()
        {
            OnNext(this, EventArgs.Empty);
        }

        public void Enter()
        {
            if (SubmitOnEnter)
            {
                OnTextSubmitted?.Invoke(this, EventArgs.Empty);

                Close();
            }
            else
            {
                string enterString = "\n";

                m_CaretPosition = InputField.caretPosition;

                InputField.text = InputField.text.Insert(m_CaretPosition, enterString);
                m_CaretPosition += enterString.Length;

                UpdateCaretPosition(m_CaretPosition);
            }

        }

        public void Shift(bool newShiftState)
        {
            m_IsShifted = newShiftState;
            OnKeyboardShifted(m_IsShifted);

            if (m_IsCapslocked && !newShiftState)
            {
                m_IsCapslocked = false;
            }
        }

        public void CapsLock(bool newCapsLockState)
        {
            m_IsCapslocked = newCapsLockState;
            Shift(newCapsLockState);
        }

        public void Space()
        {
            m_CaretPosition = InputField.caretPosition;
            InputField.text = InputField.text.Insert(m_CaretPosition++, " ");

            UpdateCaretPosition(m_CaretPosition);
        }

        public void Tab()
        {
            string tabString = "\t";

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, tabString);
            m_CaretPosition += tabString.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        public void MoveCaretLeft()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition > 0)
            {
                --m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        public void MoveCaretRight()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition < InputField.text.Length)
            {
                ++m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        public void Close()
        {
            OnClosed(this, EventArgs.Empty);            
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            ResetKeyboardState();
            if (InputField.caretPosition != 0)
            {
                InputField.MoveTextStart(false);
            }
            InputField.text = "";
            m_CaretPosition = InputField.caretPosition;
        }

        #endregion

        public void SetScaleSizeValues(float minScale, float maxScale, float minDistance, float maxDistance)
        {
            m_MinScale = minScale;
            m_MaxScale = maxScale;
            m_MinDistance = minDistance;
            m_MaxDistance = maxDistance;
        }

        #region Keyboard Layout Modes

        public void ShowAlphaKeyboard()
        {
            AlphaKeyboard.gameObject.SetActive(true);
            m_LastKeyboardLayout = LayoutType.Alpha;
        }


        private void DisableAllKeyboards()
        {
            AlphaKeyboard.gameObject.SetActive(false);
        }

        private void ResetKeyboardState()
        {
            CapsLock(false);
        }

        #endregion Keyboard Layout Modes

        
        private void IndicateActivity()
        {
            ResetClosingTime();
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
        }

        private void ResetClosingTime()
        {
            if (CloseOnInactivity)
            {
                _closingTime = Time.time + CloseOnInactivityTime;
            }
        }

        private void CheckForCloseOnInactivityTimeExpired()
        {
            if (Time.time > _closingTime && CloseOnInactivity)
            {
                Close();
            }
        }
    }
}
