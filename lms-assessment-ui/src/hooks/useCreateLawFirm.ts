import { useMutation, useQueryClient } from "@tanstack/react-query";
import useApi from "~/hooks/useApi";
import type { CreateLawFirmRequest } from "~/types/law-firm-types";

export default function useCreateLawFirm() {
  const { createLawFirm } = useApi();
  const queryClient = useQueryClient();

  const createLawFirmMutation = useMutation({
    mutationFn: (values: CreateLawFirmRequest) => createLawFirm(values),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["getLawFirms"],
      });
    },
  });

  return {
    createLawFirmAsync: createLawFirmMutation.mutateAsync,
    isSubmitting: createLawFirmMutation.isPending,
  };
}
