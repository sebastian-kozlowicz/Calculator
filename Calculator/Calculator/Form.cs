using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Calculator
{
    public partial class Form : System.Windows.Forms.Form
    {
        #region Class fields

        /// <summary>
        /// Store one of following operations character: addition (+), subtraction (-), multiplication (*), division (/)
        /// </summary>
        string operationType = string.Empty;
        /// <summary>
        /// Store float point value number divided with comma character
        /// </summary>
        string floatValueNumber = string.Empty;
        /// <summary>
        /// Helper variable stores float point value number
        /// </summary>
        string firstNumber = string.Empty;
        /// <summary>
        /// result of operations
        /// </summary>
        double result = 0;
        /// <summary>
        /// True if each operation button has clicked
        /// </summary>
        bool isDivideButtonClicked = false;
        bool isTimesButtonClicked = false;
        bool isMinusButtonClicked = false;
        bool isPlusButtonClicked = false;
        /// <summary>
        /// True if two or more numbers have entered
        /// </summary>
        bool atLeastTwoNumbersEntered = false;
        /// <summary>
        /// True if number is divide by zero
        /// </summary>
        bool divisionByZero = false;
        /// <summary>
        /// True if comma button has clicked
        /// </summary>
        bool commaButtonClicked = false;
        /// <summary>
        /// True if comma sign is enterd has sequence
        /// </summary>
        bool commaSignIsEntered = false;
        /// <summary>
        /// True if plus, minus, times or divide button has clicked
        /// </summary>
        bool operationButtonClicked = false;
        /// <summary>
        /// Stores entered numbers with operation chars
        /// </summary>
        List<string> enteredSequence = new List<string>();

        #endregion

        #region Constructor

        public Form()
        {
            InitializeComponent();
            ///Add methods to click event
            this.EqualsButton.Click += new System.EventHandler(this.SaveHistory);
            this.EqualsButton.Click += new System.EventHandler(this.DeleteSequence);
        }

        #endregion

        #region Control methods

        /// <summary>
        /// Adds character to list
        /// </summary>
        /// <param name="character">number or operation character</param>
        private void AddSequence(string character)
        {
            enteredSequence.Add(character);
        }

        /// <summary>
        /// Removes last character from list
        /// </summary>
        private void RemoveLastCharacterInSequence()
        {
            if (enteredSequence.Any()) //prevent IndexOutOfRangeException for empty list
                enteredSequence.RemoveAt(enteredSequence.Count - 1);
        }

        /// <summary>
        /// Remove latest clicked number from InputText TextBox
        /// </summary>
        /// <returns>
        /// String without last character in the chain or empty string 
        /// </returns>
        private string RemoveLastCharacterInInputText()
        {
            string text = InputText.Text;

            if (text.Any()) //prevent IndexOutOfRangeException for empty string
            {
                text = text.Remove(text.Length - 1);
                return text;
            }
            return text = string.Empty;
        }

        /// <summary>
        /// Clears PreviewOperations Label, InputText TextBox
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void DeleteSequence(object sender, EventArgs e)
        {
            enteredSequence.Clear();
            previewOperations.Text = string.Empty;

            //Making sure EqualsButton_Click method won't be executed from OperationButtons_Click method if two numbers are not entered after any operation button click
            atLeastTwoNumbersEntered = false;

            if (!divisionByZero)
                //Add result to sequence to allow user start another operation with first number
                //Do not add if number was divided by zero
                enteredSequence.Add(result.ToString());
        }

        /// <summary>
        /// Print entered sequence in previewOperation Label
        /// </summary>
        private void ShowEnteredSequence()
        {
            previewOperations.Text = string.Empty;
            // Iterate each index of list and fill previewOperations Label
            enteredSequence.ForEach(character => previewOperations.Text += character);
        }

        /// <summary>
        /// Add entered sequence to log history file
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void SaveHistory(object sender, EventArgs e)
        {
            if (DivisionByZeroLabel.Text == string.Empty)
            {
                // Iterate each index of list and fill the log file 
                enteredSequence.ForEach(character =>
                {
                    Trace.Write(character);
                });
                Trace.WriteLine($"={result}");
            }
        }

        #endregion

        #region Main logic methods

        /// <summary>
        /// Add clicked number to entered number sequence
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void ButtonNumber_Click(object sender, EventArgs e)
        {
            Button number = sender as Button;

            // Clear InputText TextBox to insert another number after any operation button click
            if (operationButtonClicked)
                InputText.Text = string.Empty;

            // Check if comma button has clicked
            if (commaButtonClicked)
            {
                // Check if comma sign is not entered yet in the sequence
                if (!commaSignIsEntered)
                {
                    // Concatenate remembered first entered number with second and separate them with comma sign 
                    floatValueNumber = firstNumber.ToString() + "," + number.Text;
                    commaSignIsEntered = true;
                }
                else
                {
                    // Add number at the end of the string chain after comma sign
                    floatValueNumber = firstNumber.ToString() + number.Text;
                }

                // Remember float point value  number as first number to enable concatenate second number in next button number click
                firstNumber = floatValueNumber;
                InputText.Text = string.Empty;
                InputText.Text += floatValueNumber.ToString();
            }

            // If number is divided by zero, switch atLeastTwoNumbersEntered to false to disable EqualsButton_Click method execute from OperationButtons_Click method
            if (divisionByZero)
            {
                atLeastTwoNumbersEntered = false;
                // Switch divisionByZero to false and clear DivisionByZeroLabel 
                divisionByZero = false;
                DivisionByZeroLabel.Text = string.Empty;
            }
            else
            {
                atLeastTwoNumbersEntered = true;
            }

            // If only zero is entered in InputText TextBox, don't add anoter one
            if (InputText.Text.Equals("0"))
            {
                InputText.Text = string.Empty;

                // If list is empty add zero to list
                if (!enteredSequence.Any())
                {
                    AddSequence(number.Text);
                }
            }
            else
            { 
                AddSequence(number.Text);
            }
               
            ShowEnteredSequence();

            if (!commaButtonClicked)
                InputText.Text += number.Text;

            isTimesButtonClicked = false;
            isDivideButtonClicked = false;
            isMinusButtonClicked = false;
            isPlusButtonClicked = false;
            operationButtonClicked = false;
        }

        /// <summary>
        /// Add operation button sign to sequence 
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void OperationButtons_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            //Replace operation type character if operation button is clicked again
            if (operationButtonClicked)
                RemoveLastCharacterInSequence();

            AddSequence(button.Text);
            DivisionByZeroLabel.Text = string.Empty;

            ShowEnteredSequence();

            // Execute equals operation if two at least two numbers are entered and operation button is clicked afterward
            if (atLeastTwoNumbersEntered && !operationButtonClicked)
            {
                EqualsButton_Click(sender, e);
            }

            result = double.Parse(InputText.Text);
            operationType = button.Text;

            commaButtonClicked = false;
            commaSignIsEntered = false;
            operationButtonClicked = true;
        }

        /// <summary>
        /// Gives result of the operations
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void EqualsButton_Click(object sender, EventArgs e)
        {
            switch (operationType)
            {
                case "+":
                    result += double.Parse(InputText.Text);
                    InputText.Text = result.ToString();
                    break;
                case "-":
                    result -= double.Parse(InputText.Text);
                    InputText.Text = result.ToString();
                    break;
                case "*":
                    result *= double.Parse(InputText.Text);
                    InputText.Text = result.ToString();
                    break;
                case "/":
                    try
                    {
                        // Check if number is not divided by zero
                        if (double.Parse(InputText.Text) == 0)
                        {
                            divisionByZero = true;
                            throw new InvalidOperationException("Don't divide by zero!");
                        }
                        result /= double.Parse(InputText.Text);
                        InputText.Text = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        DivisionByZeroLabel.Text = ex.Message;
                        // Clear whole entered sequence
                        DeleteSequence(sender, e);
                    }
                    break;
            }
        }

        #endregion

        #region Special button methods

        /// <summary>
        /// Adds comma sing to sequence
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void CommaButton_Click(object sender, EventArgs e)
        {
            // Disable addition more than one comma sing in one number
            if (!commaButtonClicked)
            {
                // Remember entered number before comma sing
                firstNumber = InputText.Text; 
                AddSequence(",");
                ShowEnteredSequence();
                commaButtonClicked = true;
            }
        }

        /// <summary>
        /// Clear InputText TextBox and enteredSequence list
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void CButton_Click(object sender, EventArgs e)
        {
            InputText.Text = "0";
            previewOperations.Text = string.Empty;
            DivisionByZeroLabel.Text = string.Empty;
            result = 0;
            DeleteSequence(sender, e);
        }

        /// <summary>
        /// Delete last entered character from sequence
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (InputText.Text != string.Empty && !operationButtonClicked)
            {
                // Not call this method if operation button has clicked to make sure sequence is evenly delete
                InputText.Text = RemoveLastCharacterInInputText();
            }

            // Delete last character from list
            RemoveLastCharacterInSequence();
            ShowEnteredSequence();
            operationButtonClicked = false;
        }

        /// <summary>
        /// Print history operations in new window
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void HistoryButton_Click(object sender, EventArgs e)
        {
            string text;
            // Open existing file for reading
            var fileStream = new FileStream("history.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                // Read file
                text = streamReader.ReadToEnd();

                // Print message
                if (text == string.Empty)
                    MessageBox.Show("There's no history yet");
                else
                    MessageBox.Show(text);
            }
        }

        #endregion

        #region Operation Buttons

        /// <summary>
        /// Disable double click of divide button and call OperationButtons_Click method
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void DivideButton_Click(object sender, EventArgs e)
        {
            isTimesButtonClicked = false;
            isMinusButtonClicked = false;
            isPlusButtonClicked = false;

            if (!isDivideButtonClicked)
            {
                isDivideButtonClicked = true;
                OperationButtons_Click(sender, e);
            }
        }

        /// <summary>
        /// Disable double click of times button and call OperationButtons_Click method
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void TimesButton_Click(object sender, EventArgs e)
        {
            isDivideButtonClicked = false;
            isMinusButtonClicked = false;
            isPlusButtonClicked = false;

            if (!isTimesButtonClicked)
            {
                isTimesButtonClicked = true;
                OperationButtons_Click(sender, e);
            }
        }

        /// <summary>
        /// Disable double click of minus button and call OperationButtons_Click method
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void MinusButton_Click(object sender, EventArgs e)
        {
            isDivideButtonClicked = false;
            isTimesButtonClicked = false;
            isPlusButtonClicked = false;

            if (!isMinusButtonClicked)
            {
                isMinusButtonClicked = true;
                OperationButtons_Click(sender, e);
            }
        }

        /// <summary>
        /// Disable double click of plus button and call OperationButtons_Click method
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void PlusButton_Click(object sender, EventArgs e)
        {
            isDivideButtonClicked = false;
            isTimesButtonClicked = false;
            isMinusButtonClicked = false;

            if (!isPlusButtonClicked)
            {
                isPlusButtonClicked = true;
                OperationButtons_Click(sender, e);
            }
        }

        #endregion
    }
}
