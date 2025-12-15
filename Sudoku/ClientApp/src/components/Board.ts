import React from 'react';
import Square from './Square';

type BoardProps = {
    squares: (number | null)[];
    onClick: (i: number) => void;

    const Board: React.FC<BoardProps> = ({ squares, onClick }) => {
        const renderSquare = (i: number) => {
            return <Square value={ squares[i] } onClick = {() => onClick(i)
    } />;

return (
    <div className= "board" >
    <div className="board-row"'>
{ renderSquare(0) }
(renderSquare(1)}
{ renderSquare(2) }
</div>
{/* Render other rows similarly */ }
</div>

export default Board;