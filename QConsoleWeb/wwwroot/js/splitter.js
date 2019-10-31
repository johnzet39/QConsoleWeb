// function is used for dragging and moving
function splitter(firstElementName, secondElementName, separatorName, splitterName, isPercent, direction, handler) {
    const separator = document.getElementById(separatorName);
    const first = document.getElementById(firstElementName);
    const second = document.getElementById(secondElementName);
    const splitter = document.getElementById(splitterName);

    var md;

    separator.onmousedown = onMouseDown;

    function onMouseDown(e) {
        //console.log("mouse down: " + e.clientX);
        md = {
            e,
            offsetLeft: separator.offsetLeft,
            offsetTop: separator.offsetTop,
            firstWidth: first.offsetWidth,
            secondWidth: second.offsetWidth
        };
        document.onmousemove = onMouseMove;
        document.onmouseup = () => {
            //console.log("mouse up");
            document.onmousemove = document.onmouseup = null;
        }
    }

    function onMouseMove(e) {
        //console.log("mouse move: " + e.clientX);
        var delta = {
            x: e.clientX - md.e.x,
            y: e.clientY - md.e.y
        };

        if (direction === "H") // Horizontal
        {
            // prevent negative-sized elements
            delta.x = Math.min(Math.max(delta.x, -md.firstWidth),
                md.secondWidth);

            if (isPercent) {
                var leftWidthPercent = (md.firstWidth + delta.x) / splitter.offsetWidth * 100;
                first.style.width = leftWidthPercent + "%";
                second.style.width = 100 - leftWidthPercent + "%";
            }
            else {
                separator.style.left = md.offsetLeft + delta.x + "px";
                first.style.width = (md.firstWidth + delta.x) + "px";
                second.style.width = (md.secondWidth - delta.x) + "px";
            }

        }
    }



    //// Two variables for tracking positions of the cursor
    //const drag = { x: 0, y: 0 };
    //const delta = { x: 0, y: 0 };
    ///* if present, the handler is where you move the DIV from
    //    otherwise, move the DIV from anywhere inside the DIV */
    //handler ? (handler.onmousedown = dragMouseDown) : (separator.onmousedown = dragMouseDown);

    //// function that will be called whenever the down event of the mouse is raised
    //function dragMouseDown(e) {
    //    drag.x = e.clientX;
    //    drag.y = e.clientY;
    //    document.onmousemove = onMouseMove;
    //    document.onmouseup = () => { document.onmousemove = document.onmouseup = null; }
    //}

    //// function that will be called whenever the up event of the mouse is raised
    //function onMouseMove(e) {
    //    const currentX = e.clientX;
    //    const currentY = e.clientY;

    //    delta.x = currentX - drag.x;
    //    delta.y = currentY - drag.y;

    //    const offsetLeft = separator.offsetLeft;
    //    const offsetTop = separator.offsetTop;


        
    //    let firstWidth = first.offsetWidth;
    //    let secondWidth = second.offsetWidth;
    //    if (direction === "H") // Horizontal
    //    {
    //        separator.style.left = offsetLeft + delta.x + "px";
    //        firstWidth += delta.x;
    //        secondWidth -= delta.x;
    //    }
    //    drag.x = currentX;
    //    drag.y = currentY;
    //    first.style.width = firstWidth + "px";
    //    second.style.width = secondWidth + "px";
    //}
}