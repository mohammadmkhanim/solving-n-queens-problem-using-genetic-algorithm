
function createChessboard(n, id) {
    boardSize = n;
    const chessboard = document.getElementById(id);
    chessboard.innerHTML = '';

    for (let row = 0; row < n; row++) {
        for (let col = 0; col < n; col++) {
            const square = document.createElement('div');
            id
            square.classList.add('square');
            square.classList.add(id);

            if ((row + col) % 2 === 0) {
                square.classList.add('light');
            } else {
                square.classList.add('dark');
            }
            square.style.height = "calc(" + "100% / " + n + ")"
            square.style.width = "calc(" + "100% / " + n + ")"

            chessboard.appendChild(square);
        }
    }
}

let boardSize = 0;

function setMinisterPosition(row, col, id) {
    const squares = document.getElementsByClassName(id);


    squares.item([row * boardSize + col]).classList.add('minister');
    squares.item([row * boardSize + col]).innerHTML = '<i class="fa-solid fa-chess-queen text-white text-3xl flex justify-center items-center"></i>';
}

function addClassToElement(arrays) {
    arrays.forEach((array) => {
        let element = document.getElementById(array[0]);
        if (element) {
            element.classList.add(array[1]);
        }
    });
}

function removeClassFromElement(arrays) {
    arrays.forEach((array) => {
        let element = document.getElementById(array[0]);
        if (element) {
            element.classList.remove(array[1]);
        }
    });
}

