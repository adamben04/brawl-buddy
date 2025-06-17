import React from 'react';

const BrawlerList: React.FC<{ brawlerIds: number[] }> = ({ brawlerIds }) => {
  return (
    <div className="brawler-list">
      {brawlerIds.map((id) => (
        <div key={id} className="brawler-item">
          <img
            src={`https://cdn.brawlify.com/brawlers/borderless/${id}.png`}
            alt={`Brawler ${id}`}
            className="brawler-image"
            onError={(e) => {
              e.currentTarget.src = '/path/to/placeholder.png'; // Fallback image
            }}
          />
        </div>
      ))}
    </div>
  );
};

export default BrawlerList;