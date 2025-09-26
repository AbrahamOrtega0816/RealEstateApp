import React from "react";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  showPageInfo?: boolean;
}

/**
 * Reusable pagination component with ellipsis support
 */
export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange,
  showPageInfo = true,
}) => {
  // Ensure we have valid numbers and at least 1 page for display
  const safeTotalPages =
    Number.isFinite(totalPages) && totalPages > 0 ? totalPages : 1;
  const safeCurrentPage =
    Number.isFinite(currentPage) && currentPage > 0 ? currentPage : 1;

  const displayTotalPages = Math.max(safeTotalPages, 1);
  const displayCurrentPage = Math.max(
    Math.min(safeCurrentPage, displayTotalPages),
    1
  );

  // Function to generate page numbers with ellipsis
  const getPageNumbers = () => {
    const maxPagesToShow = 5; // Maximum number of page buttons to show
    let pages: (number | "ellipsis")[] = [];

    // Always show the first page
    pages.push(1);

    if (displayTotalPages <= maxPagesToShow) {
      // If there are few pages, show all
      for (let i = 2; i <= displayTotalPages; i++) {
        pages.push(i);
      }
    } else {
      // If user is near the beginning
      if (displayCurrentPage <= 3) {
        pages.push(2, 3);
        pages.push("ellipsis");
        pages.push(displayTotalPages);
      }
      // If user is near the end
      else if (displayCurrentPage >= displayTotalPages - 2) {
        pages.push("ellipsis");
        pages.push(
          displayTotalPages - 2,
          displayTotalPages - 1,
          displayTotalPages
        );
      }
      // If user is in the middle
      else {
        pages.push("ellipsis");
        pages.push(
          displayCurrentPage - 1,
          displayCurrentPage,
          displayCurrentPage + 1
        );
        pages.push("ellipsis");
        pages.push(displayTotalPages);
      }
    }

    return pages;
  };

  const pageNumbers = getPageNumbers();

  return (
    <div className="flex flex-col sm:flex-row items-center justify-between gap-4 py-4">
      {/* Mobile view */}
      <div className="join sm:hidden">
        <button
          className={`join-item btn ${
            displayCurrentPage === 1 ? "btn-disabled" : ""
          }`}
          onClick={() => onPageChange(displayCurrentPage - 1)}
          disabled={displayCurrentPage === 1}
        >
          «
        </button>
        <button className="join-item btn">Page {displayCurrentPage}</button>
        <button
          className={`join-item btn ${
            displayCurrentPage === displayTotalPages ? "btn-disabled" : ""
          }`}
          onClick={() => onPageChange(displayCurrentPage + 1)}
          disabled={displayCurrentPage === displayTotalPages}
        >
          »
        </button>
      </div>

      {/* Desktop view */}
      <div className="hidden sm:flex items-center justify-between w-full">
        {showPageInfo && (
          <div className="text-sm text-base-content/70">
            Showing page{" "}
            <span className="font-semibold text-base-content">
              {displayCurrentPage}
            </span>{" "}
            of{" "}
            <span className="font-semibold text-base-content">
              {displayTotalPages}
            </span>
          </div>
        )}

        <div className="join">
          {/* Previous button */}
          <button
            className={`join-item btn ${
              displayCurrentPage === 1 ? "btn-disabled" : ""
            }`}
            disabled={displayCurrentPage === 1}
            onClick={() => onPageChange(displayCurrentPage - 1)}
          >
            «
          </button>

          {/* Page numbers */}
          {pageNumbers.map((page, index) => {
            if (page === "ellipsis") {
              return (
                <button
                  key={`ellipsis-${index}`}
                  className="join-item btn btn-disabled"
                  disabled
                >
                  ...
                </button>
              );
            }

            return (
              <button
                key={page}
                className={`join-item btn ${
                  page === displayCurrentPage ? "btn-primary" : ""
                }`}
                onClick={() => onPageChange(page as number)}
              >
                {page}
              </button>
            );
          })}

          {/* Next button */}
          <button
            className={`join-item btn ${
              displayCurrentPage === displayTotalPages ? "btn-disabled" : ""
            }`}
            disabled={displayCurrentPage === displayTotalPages}
            onClick={() => onPageChange(displayCurrentPage + 1)}
          >
            »
          </button>
        </div>
      </div>
    </div>
  );
};
