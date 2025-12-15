import React from 'react';

type SquareProps = {
    value: number | null;
    onClick: () => void;
};

const Square: React.FC<SquareProps> = ({ value, onClick }) => {
    return (
        <button className="square" onClick={onClick}>
            { value }
        </button>
    );
};

export default Square;