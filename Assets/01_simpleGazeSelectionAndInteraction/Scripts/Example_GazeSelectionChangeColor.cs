
// -----------------------------------------------------------------------
//
// (c) Copyright 1997-2016, SensoMotoric Instruments GmbH
// 
// Permission  is  hereby granted,  free  of  charge,  to any  person  or
// organization  obtaining  a  copy  of  the  software  and  accompanying
// documentation  covered  by  this  license  (the  "Software")  to  use,
// reproduce,  display, distribute, execute,  and transmit  the Software,
// and  to  prepare derivative  works  of  the  Software, and  to  permit
// third-parties to whom the Software  is furnished to do so, all subject
// to the following:
// 
// The  copyright notices  in  the Software  and  this entire  statement,
// including the above license  grant, this restriction and the following
// disclaimer, must be  included in all copies of  the Software, in whole
// or  in part, and  all derivative  works of  the Software,  unless such
// copies   or   derivative   works   are   solely   in   the   form   of
// machine-executable  object   code  generated  by   a  source  language
// processor.
// 
// THE  SOFTWARE IS  PROVIDED  "AS  IS", WITHOUT  WARRANTY  OF ANY  KIND,
// EXPRESS OR  IMPLIED, INCLUDING  BUT NOT LIMITED  TO THE  WARRANTIES OF
// MERCHANTABILITY,   FITNESS  FOR  A   PARTICULAR  PURPOSE,   TITLE  AND
// NON-INFRINGEMENT. IN  NO EVENT SHALL  THE COPYRIGHT HOLDERS  OR ANYONE
// DISTRIBUTING  THE  SOFTWARE  BE   LIABLE  FOR  ANY  DAMAGES  OR  OTHER
// LIABILITY, WHETHER  IN CONTRACT, TORT OR OTHERWISE,  ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE  SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// -----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using SMI;
namespace SMI.Example
{
    //Inherit GazeMonobehaviour Class
    public class Example_GazeSelectionChangeColor : GazeMonoBehaviour
    {
        [Tooltip("The color of the object when the player gazes at.")]
        [SerializeField]
        private Color highLightColor;

        [Tooltip("The color of the object when the player is not gazing at.")]
        [SerializeField]
        private Color normalColor;

        //For color transition
        private Color destinationColor;


        void Start()
        {
            destinationColor = normalColor;
        }

        void Update()
        {
            //Smooth color transition by leap
            GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, destinationColor, 0.2f);
        }

        //OnGazeStay is called while the gaze stays on the object (every frame)
        public override void OnGazeStay(RaycastHit hitInformation)
        {
            base.OnGazeStay(hitInformation);
            destinationColor = highLightColor;
        }

        //OnGazeExit is called once when the gaze exits the object
        public override void OnGazeExit()
        {
            base.OnGazeExit();
            destinationColor = normalColor;
        }

        //OnGazeEnter is called once when the gaze enters the object
        public override void OnGazeEnter(RaycastHit hitInformation)
        {
            base.OnGazeEnter(hitInformation);
        }
    }
}